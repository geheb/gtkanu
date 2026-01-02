using GtKanu.Core.Converter;
using GtKanu.Core.Database;
using GtKanu.Core.Entities;
using GtKanu.Core.Extensions;
using GtKanu.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GtKanu.Core.Repositories;

public sealed class Clubhouse
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly AppDbContext _dbContext;

    public Clubhouse(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClubhouseBookingStatus> CreateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<ClubhouseBooking>();

        var existsBooking = await dbSet.AnyAsync(e =>
            ((e.Start >= dto.Start && e.Start <= dto.End) || (e.End >= dto.Start && e.End <= dto.End) || (e.End >= dto.End && e.Start <= dto.Start)),
            cancellationToken);

        if (existsBooking)
        {
            return ClubhouseBookingStatus.Exists;
        }

        var entity = dto.ToEntity();
        entity.Id = _pkGenerator.Generate();

        await dbSet.AddAsync(entity, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 
            ? ClubhouseBookingStatus.Success 
            : ClubhouseBookingStatus.Failed;
    }

    public async Task<ClubhouseBookingDto?> FindBooking(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<ClubhouseBooking>().FindAsync([id], cancellationToken);
        if (entity == null) return null;

        return new ClubhouseBookingDto(entity, new());
    }

    public async Task<ClubhouseBookingStatus> UpdateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<ClubhouseBooking>();

        var entity = await dbSet.FindAsync([dto.Id], cancellationToken);
        if (entity == null) return ClubhouseBookingStatus.NotFound;

        var existsBooking = await dbSet.AnyAsync(e =>
            e.Id != entity.Id &&
            ((e.Start >= dto.Start && e.Start <= dto.End) || (e.End >= dto.Start && e.End <= dto.End) || (e.End >= dto.End && e.Start <= dto.Start)),
            cancellationToken);

        if (existsBooking)
        {
            return ClubhouseBookingStatus.Exists;
        }

        var count = 0;
        if (entity.SetValue(e => e.Start, dto.Start)) count++;
        if (entity.SetValue(e => e.End, dto.End)) count++;
        if (entity.SetValue(e => e.Title, dto.Title)) count++;
        if (entity.SetValue(e => e.Description, dto.Description)) count++;

        if (count == 0) return ClubhouseBookingStatus.Success;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0
            ? ClubhouseBookingStatus.Success
            : ClubhouseBookingStatus.Failed;
    }

    public async Task<ClubhouseBookingDto[]> GetBookingList(bool showExpired, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var entities = await _dbContext.Set<ClubhouseBooking>()
            .AsNoTracking()
            .Where(e => (showExpired ? e.Start < now : e.End > now))
            .OrderByDescending(e => e.Start)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new ClubhouseBookingDto(e, dc)).ToArray();
    }

    public async Task<bool> DeleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<ClubhouseBooking>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _dbContext.Remove(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
