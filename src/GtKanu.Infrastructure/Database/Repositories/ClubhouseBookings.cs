using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class ClubhouseBookings : IClubhouseBookings
{
    private readonly AppDbContext _dbContext;

    public ClubhouseBookings(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClubhouseBookingStatus> CreateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken)
    {
        var existsBooking = await _dbContext.ClubhouseBookings.AnyAsync(e =>
            ((e.Start >= dto.Start && e.Start <= dto.End) || (e.End >= dto.Start && e.End <= dto.End) || (e.End >= dto.End && e.Start <= dto.Start)),
            cancellationToken);

        if (existsBooking)
        {
            return ClubhouseBookingStatus.Exists;
        }

        var entity = new ClubhouseBooking();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        _dbContext.Add(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 
            ? ClubhouseBookingStatus.Success 
            : ClubhouseBookingStatus.Failed;
    }

    public async Task<ClubhouseBookingDto?> FindBooking(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ClubhouseBookings
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity?.ToDto(new());
    }

    public async Task<ClubhouseBookingStatus> UpdateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ClubhouseBookings.FindAsync([dto.Id], cancellationToken);
        if (entity is null)
        {
            return ClubhouseBookingStatus.NotFound;
        }

        var existsBooking = await _dbContext.ClubhouseBookings.AnyAsync(e =>
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

        if (count == 0)
        {
            return ClubhouseBookingStatus.Success;
        }

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0
            ? ClubhouseBookingStatus.Success
            : ClubhouseBookingStatus.Failed;
    }

    public async Task<ClubhouseBookingDto[]> GetAll(bool showExpired, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var entities = await _dbContext.ClubhouseBookings
            .AsNoTracking()
            .Where(e => (showExpired ? e.Start < now : e.End > now))
            .OrderByDescending(e => e.Start)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return [.. entities.Select(e => e.ToDto(dc))];
    }

    public async Task<bool> DeleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ClubhouseBookings
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _dbContext.Remove(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
