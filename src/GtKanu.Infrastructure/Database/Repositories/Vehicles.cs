namespace GtKanu.Infrastructure.Database.Repositories;

using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

internal sealed class Vehicles : IVehicles, IDisposable
{
    private readonly SemaphoreSlim _bookingSemaphore = new SemaphoreSlim(1, 1);
    private readonly AppDbContext _dbContext;

    public Vehicles(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose() => _bookingSemaphore.Dispose();

    public async Task<VehicleStatus> CreateVehicle(VehicleDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Vehicle>();

        var exists = await dbSet.AnyAsync(e => e.Identifier == dto.Identifier && e.IsInUse, cancellationToken);
        if (exists) return VehicleStatus.Exists;

        var entity = new Vehicle();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        dbSet.Add(entity);

        dto.Id = entity.Id;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? VehicleStatus.Success : VehicleStatus.PersistFailed;
    }

    public async Task<VehicleDto[]> GetAllVehicles(CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Vehicle>();
        var entities = await dbSet.OrderBy(e => e.Name).ToArrayAsync(cancellationToken);
        return entities.Select(e => e.ToDto()).ToArray();
    }

    public async Task<VehicleDto[]> GetVehiclesInUseOnly(CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Vehicle>();
        var entities = await dbSet.Where(e => e.IsInUse).OrderBy(e => e.Name).ToArrayAsync(cancellationToken);
        return entities.Select(e => e.ToDto()).ToArray();
    }

    public async Task<VehicleBookingStatus> CreateBooking(CreateVehicleBookingDto dto, CancellationToken cancellationToken)
    {
        if (!await _bookingSemaphore.WaitAsync(TimeSpan.FromMinutes(1), cancellationToken)) return VehicleBookingStatus.Timeout;

        try
        {
            var dbSet = _dbContext.Set<VehicleBooking>();

            var existsBooking = await dbSet.AnyAsync(e => 
                e.VehicleId == dto.VehicleId && 
                e.CancelledOn == null && 
                ((e.Start >= dto.Start && e.Start <= dto.End) || (e.End >= dto.Start && e.End <= dto.End) || (e.End >= dto.End && e.Start <= dto.Start)),
                cancellationToken);

            if (existsBooking)
            {
                return VehicleBookingStatus.AlreadyBooked;
            }

            var entity = new VehicleBooking
            {
                Id = _dbContext.GeneratePk(),
                BookedOn = DateTimeOffset.UtcNow,
                Start = dto.Start,
                End = dto.End,
                VehicleId = dto.VehicleId,
                UserId = dto.UserId,
                Purpose = dto.Purpose
            };

            dbSet.Add(entity);

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? VehicleBookingStatus.Success : VehicleBookingStatus.Failed;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<VehicleDto?> FindVehicle(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<Vehicle>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null) return null;

        return entity.ToDto();
    }

    public async Task<VehicleStatus> UpdateVehicle(VehicleDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Vehicle>();

        var existent = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Identifier == dto.Identifier && e.IsInUse, cancellationToken);

        if (existent is not null && existent.Id != dto.Id) return VehicleStatus.Exists;

        var entity = await dbSet.FirstOrDefaultAsync(e => e.Id == dto.Id, cancellationToken);
        if (entity is null) return VehicleStatus.NotFound;

        var count = 0;
        if (entity.SetValue(e => e.Name, dto.Name?.Trim())) count++;
        if (entity.SetValue(e => e.Identifier, dto.Identifier?.Trim())) count++;
        if (entity.SetValue(e => e.IsInUse, dto.IsInUse)) count++;

        if (count < 1) return VehicleStatus.Success;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? VehicleStatus.Success : VehicleStatus.PersistFailed;
    }

    public async Task<VehicleBookingDto[]> GetBookings(bool showExpired, Guid? userId, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        VehicleBooking[] entities;

        if (userId is not null)
        {
            entities = await _dbContext.Set<VehicleBooking>()
                .AsNoTracking()
                .Include(e => e.Vehicle)
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .ToArrayAsync(cancellationToken);
        }
        else
        {
            entities = await _dbContext.Set<VehicleBooking>()
                .AsNoTracking()
                .Include(e => e.Vehicle)
                .Include(e => e.User)
                .Where(e => showExpired ? e.End < now : e.End > now)
                .ToArrayAsync(cancellationToken);
        }

        var dc = new GermanDateTimeConverter();

        var result = entities.Select(e => e.ToDto(dc)).ToArray();
        
        return result.Where(r => r.Start >= now).OrderBy(r => r.Start)
            .Concat(result.Where(r => r.Start < now).OrderByDescending(r => r.Start))
            .ToArray();
    }

    public async Task<bool> DeleteBooking(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<VehicleBooking>()
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);

        if (entity == null ||
            entity.ConfirmedOn is not null)
        {
            return false;
        }

        _dbContext.Remove(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<VehicleBooking>();

        var entity = await dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity == null)
        {
            return false;
        }

        var currentId = entity.Id;
        var currentVehicleId = entity.VehicleId;
        var currentStart = entity.Start;
        var currentEnd = entity.End;

        var existsBooking = await dbSet.AnyAsync(e =>
            e.VehicleId == currentVehicleId &&
            e.CancelledOn == null &&
            e.Id != currentId &&
            ((e.Start >= currentStart && e.Start <= currentEnd) || (e.End >= currentStart && e.End <= currentEnd) || (e.End >= currentEnd && e.Start <= currentStart)),
            cancellationToken);

        if (existsBooking)
        {
            return false;
        }


        entity.ConfirmedOn = DateTimeOffset.UtcNow;
        entity.CancelledOn = null;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CancelBooking(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<VehicleBooking>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        entity.ConfirmedOn = null;
        entity.CancelledOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
