using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class Boats : IBoats, IDisposable
{
    private readonly SemaphoreSlim _bookingSemaphore = new SemaphoreSlim(1, 1);
    private readonly AppDbContext _dbContext;

    public Boats(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose() => _bookingSemaphore.Dispose();

    public async Task<BoatStatus> Create(BoatDto dto, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Boats.AnyAsync(e => e.Identifier == dto.Identifier && e.IsLocked == false, cancellationToken);
        if (exists)
        {
            return BoatStatus.Exists;
        }

        var entity = new Boat();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        _dbContext.Add(entity);

        dto.Id = entity.Id;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? BoatStatus.Success : BoatStatus.PersistFailed;
    }

    public async Task<BoatStatus> Update(BoatDto dto, CancellationToken cancellationToken)
    {
        var update = new Boat();
        update.FromDto(dto);

        var existent = await _dbContext.Boats
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Identifier == update.Identifier && e.IsLocked == false, cancellationToken);

        if (existent is not null && existent.Id != update.Id) return BoatStatus.Exists;

        var entity = await _dbContext.Boats.FirstOrDefaultAsync(e => e.Id == update.Id, cancellationToken);
        if (entity is null) return BoatStatus.NotFound;

        var count = 0;
        if (entity.SetValue(e => e.Name, update.Name)) count++;
        if (entity.SetValue(e => e.Identifier, update.Identifier)) count++;
        if (entity.SetValue(e => e.IsLocked, update.IsLocked)) count++;
        if (entity.SetValue(e => e.Location, update.Location)) count++;
        if (entity.SetValue(e => e.MaxRentalDays, update.MaxRentalDays)) count++;
        if (entity.SetValue(e => e.Description, update.Description)) count++;

        if (count < 1) return BoatStatus.Success;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? BoatStatus.Success : BoatStatus.PersistFailed;
    }

    public async Task<BoatDto?> FindBoat(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Boats
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity?.ToDto();
    }

    public async Task<BoatRentalListDto[]> GetRentalList(CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Boats
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .Select(e => new
            {
                boat = e,
                count = e.BoatRentals == null ? 0 : e.BoatRentals.Count()
            })
            .ToArrayAsync(cancellationToken);

        if (entities.Length == 0)
        {
            return [];
        }

        var result = new List<BoatRentalListDto>();

        foreach (var entity in entities)
        {
            result.Add(new() { Boat = entity.boat.ToDto(), Count = entity.count });
        }

        return [.. result];
    }

    public async Task<BoatRentalListDto[]> GetMyRentalList(Guid userId, CancellationToken cancellationToken)
    {
        var userRentals = await _dbContext.BoatRentals
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .ToArrayAsync(cancellationToken);

        if (userRentals.Length == 0)
        {
            return [];
        }

        var boatCount = userRentals
            .GroupBy(e => e.BoatId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var boatIds = boatCount.Keys.ToArray();

        var userBoats = await _dbContext.Boats
            .AsNoTracking()
            .Where(e => boatIds.Contains(e.Id))
            .OrderBy(e => e.Name)
            .ToArrayAsync(cancellationToken);

        var result = new List<BoatRentalListDto>();

        foreach (var e in userBoats)
        {
            result.Add(new() { Boat = e.ToDto(), Count = boatCount[e.Id] });
        }

        return [.. result];
    }

    public async Task<BoatRentalDto[]> GetRentals(Guid boatId, bool activeOnly, CancellationToken cancellationToken)
    {       
        var now = DateTimeOffset.UtcNow;

        var entities = await _dbContext.BoatRentals
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.BoatId == boatId && (!activeOnly || e.End > now))
            .OrderBy(e => e.Start)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        return
        [
            .. entities.Where(r => r.Start >= now).OrderBy(r => r.Start).Select(e => e.ToDto(dc)),
            .. entities.Where(r => r.Start < now).OrderByDescending(r => r.Start).Select(e => e.ToDto(dc))
        ];
    }

    public async Task<BoatRentalDto?> GetLastRental(Guid boatId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.BoatRentals
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.BoatId == boatId && e.CancelledOn == null)
            .OrderByDescending(e => e.End)
            .FirstOrDefaultAsync(cancellationToken);

        return entity?.ToDto(new());
    }

    public async Task<BoatRentalStatus> CreateRental(CreateBoatRentalDto dto, CancellationToken cancellationToken)
    {
        if (!await _bookingSemaphore.WaitAsync(TimeSpan.FromMinutes(1), cancellationToken))
        {
            return BoatRentalStatus.Timeout;
        }

        try
        {
            var existsBooking = await _dbContext.BoatRentals.AnyAsync(e =>
                e.BoatId == dto.BoatId &&
                e.CancelledOn == null &&
                ((e.Start >= dto.Start && e.Start <= dto.End) || (e.End >= dto.Start && e.End <= dto.End) || (e.End >= dto.End && e.Start <= dto.Start)),
                cancellationToken);

            if (existsBooking)
            {
                return BoatRentalStatus.AlreadyBooked;
            }

            var entity = new BoatRental();
            entity.FromDto(dto);
            entity.Id = _dbContext.GeneratePk();

            _dbContext.Add(entity);

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? BoatRentalStatus.Success : BoatRentalStatus.Failed;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<bool> StopRental(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.BoatRentals.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return false;
        }

        var dc = new GermanDateTimeConverter();
        var now = dc.ToLocal(DateTimeOffset.UtcNow);
        var end = DateOnly.FromDateTime(now.Date).ToDateTime(TimeOnly.MaxValue);

        entity.End = dc.ToUtc(end);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CancelRental(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.BoatRentals.FindAsync([id], cancellationToken);
        if (entity is null || entity.CancelledOn is not null)
        {
            return false;
        }

        entity.CancelledOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<MyBoatRentalListDto[]> GetMyRentals(Guid userId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.BoatRentals
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Start)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        var now = DateTimeOffset.UtcNow;
        return
        [
            .. entities.Where(r => r.Start >= now).OrderBy(r => r.Start).Select(e => e.ToMyListDto(dc)),
            .. entities.Where(r => r.Start < now).OrderByDescending(r => r.Start).Select(e => e.ToMyListDto(dc))
        ];
    }
}
