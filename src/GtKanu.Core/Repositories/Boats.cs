using GtKanu.Core.Converter;
using GtKanu.Core.Database;
using GtKanu.Core.Entities;
using GtKanu.Core.Extensions;
using GtKanu.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Core.Repositories;

public sealed class Boats : IDisposable
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly SemaphoreSlim _bookingSemaphore = new SemaphoreSlim(1, 1);
    private readonly AppDbContext _dbContext;

    public Boats(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose()
    {
        _bookingSemaphore.Dispose();
    }

    public async Task<BoatStatus> Create(BoatDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Boat>();

        var exists = await dbSet.AnyAsync(e => e.Identifier == dto.Identifier && e.IsLocked == false, cancellationToken);
        if (exists)
        {
            return BoatStatus.Exists;
        }

        var entity = dto.ToEntity();
        entity.Id = _pkGenerator.Generate();

        await dbSet.AddAsync(entity, cancellationToken);

        dto.Id = entity.Id;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? BoatStatus.Success : BoatStatus.PersistFailed;
    }

    public async Task<BoatStatus> Update(BoatDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Boat>();
        var update = dto.ToEntity();

        var existent = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Identifier == update.Identifier && e.IsLocked == false, cancellationToken);

        if (existent is not null && existent.Id != update.Id) return BoatStatus.Exists;

        var entity = await dbSet.FirstOrDefaultAsync(e => e.Id == update.Id, cancellationToken);
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
        var dbSet = _dbContext.Set<Boat>();

        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        return new BoatDto(entity);
    }

    public async Task<BoatRentalListDto[]> GetRentalList(CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Boat>();
        var entities = await dbSet
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .Select(e => new { boat = e, count = e.BoatRentals!.Count() })
            .ToArrayAsync(cancellationToken);

        var result = new List<BoatRentalListDto>();

        foreach(var entity in entities)
        {
            result.Add(new() { Boat = new BoatDto(entity.boat), Count = entity.count });
        }

        return result.ToArray();
    }

    public async Task<BoatRentalListDto[]> GetMyRentalList(Guid userId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Boat>();
        var entities = await dbSet
            .AsNoTracking()
            .Where(e => e.BoatRentals!.Any(e => e.UserId == userId))
            .OrderBy(e => e.Name)
            .Select(e => new { boat = e, count = e.BoatRentals!.Count(e => e.UserId == userId) })
            .ToArrayAsync(cancellationToken);

        var result = new List<BoatRentalListDto>();

        foreach (var entity in entities)
        {
            result.Add(new() { Boat = new BoatDto(entity.boat), Count = entity.count });
        }

        return result.ToArray();
    }

    public async Task<BoatRentalDto[]> GetRentals(Guid boatId, bool activeOnly, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<BoatRental>();
        
        var now = DateTimeOffset.UtcNow;

        var entities = await dbSet
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.BoatId == boatId && (!activeOnly || e.End > now))
            .OrderBy(e => e.Start)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities
            .Where(r => r.Start >= now)
            .OrderBy(r => r.Start)
            .Concat(entities.Where(r => r.Start < now).OrderByDescending(r => r.Start))
            .Select(e => new BoatRentalDto(e, dc))
            .ToArray();
    }

    public async Task<BoatRentalDto?> GetLastRental(Guid boatId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<BoatRental>();

        var entity = await dbSet
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.BoatId == boatId && e.CancelledOn == null)
            .OrderByDescending(e => e.End)
            .FirstOrDefaultAsync(cancellationToken);

        return entity is not null ? new BoatRentalDto(entity, new()) : null;
    }

    public async Task<BoatRentalStatus> CreateRental(CreateBoatRentalDto dto, CancellationToken cancellationToken)
    {
        if (!await _bookingSemaphore.WaitAsync(TimeSpan.FromMinutes(1), cancellationToken)) return BoatRentalStatus.Timeout;

        try
        {
            var dbSet = _dbContext.Set<BoatRental>();

            var existsBooking = await dbSet.AnyAsync(e =>
                e.BoatId == dto.BoatId &&
                e.CancelledOn == null &&
                ((e.Start >= dto.Start && e.Start <= dto.End) || (e.End >= dto.Start && e.End <= dto.End) || (e.End >= dto.End && e.Start <= dto.Start)),
                cancellationToken);

            if (existsBooking)
            {
                return BoatRentalStatus.AlreadyBooked;
            }

            var entity = dto.ToEntity(_pkGenerator.Generate());

            await dbSet.AddAsync(entity, cancellationToken);

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? BoatRentalStatus.Success : BoatRentalStatus.Failed;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<bool> StopRental(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<BoatRental>();

        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        var dc = new GermanDateTimeConverter();
        var now = dc.ToLocal(DateTimeOffset.UtcNow);
        var end = DateOnly.FromDateTime(now.Date).ToDateTime(TimeOnly.MaxValue);

        entity.End = new DateTimeOffset(end, now.Offset).ToUniversalTime();

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CancelRental(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<BoatRental>();

        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null || entity.CancelledOn is not null)
        {
            return false;
        }

        entity.CancelledOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<MyBoatRentalListDto[]> GetMyRentals(Guid userId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<BoatRental>();

        var now = DateTimeOffset.UtcNow;

        var entities = await dbSet
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Start)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities
            .Where(r => r.Start >= now)
            .OrderBy(r => r.Start)
            .Concat(entities.Where(r => r.Start < now).OrderByDescending(r => r.Start))
            .Select(e => new MyBoatRentalListDto(e, dc))
            .ToArray();


    }
}
