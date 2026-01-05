using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class FoodBookings : IFoodBookings
{
    private readonly AppDbContext _dbContext;

    public FoodBookings(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingFoodDto[]> GetUntilEndOfMonth(DateTime start, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var startParam = dc.ToUtc(start);
        var endParam = dc.ToUtc(new DateOnly(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)).ToDateTime(TimeOnly.MaxValue));

        var entities = await _dbContext.FoodBookings
            .AsNoTracking()
            .Include(e => e.Food)
            .Include(e => e.User)
            .Include(e => e.Invoice)
            .Where(e => e.BookedOn >= startParam && e.BookedOn <= endParam)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        return [.. entities.Select(e => e.ToDto(dc))];
    }

    public async Task<BookingFoodDto[]> GetForOneMonth(Guid userId, int year, int month, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var start = dc.ToUtc(new DateTime(year, month, 1));
        var end = dc.ToUtc(new DateOnly(year, month, DateTime.DaysInMonth(year, month)).ToDateTime(TimeOnly.MaxValue));

        var entities = await _dbContext.FoodBookings
            .AsNoTracking()
            .Include(e => e.Food)
            .Include(e => e.Invoice)
            .Where(e => e.UserId == userId && e.BookedOn >= start && e.BookedOn <= end)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        return [.. entities.Select(e => e.ToDto(dc))];
    }

    public async Task<decimal> CalcTotal(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();
        var start = dc.ToUtc(from);
        var end = dc.ToUtc(to);

        var statusCompleted = (int)BookingStatus.Completed;

        var sumPrice = await _dbContext.FoodBookings
            .AsNoTracking()
            .Where(e => e.BookedOn >= start && e.BookedOn <= end && e.Status == statusCompleted && e.InvoiceId == null)
            .SumAsync(e => e.Food!.Price * e.Count, cancellationToken);

        return sumPrice;
    }

    public async Task<BookingFoodDto[]> GetNotCancelledForOneDay(DateTime date, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();
        var start = dc.ToUtc(date.Date);
        var end = dc.ToUtc(new DateOnly(date.Year, date.Month, date.Day).ToDateTime(TimeOnly.MaxValue));

        var statusCancelled = (int)BookingStatus.Cancelled;
        var foodType = (int)FoodType.Donation;
        
        var entities = await _dbContext.FoodBookings
            .AsNoTracking()
            .Include(e => e.Food)
            .Include(e => e.User)
            .Where(e => e.BookedOn >= start && e.BookedOn <= end && e.Status != statusCancelled && e.Food!.Type != foodType)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        return [.. entities.Select(e => e.ToDto(dc))];
    }

    public async Task<BookingFoodDto[]> GetInvoiceBookings(Guid invoiceId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.FoodBookings
            .AsNoTracking()
            .Include(e => e.Food)
            .Where(e => e.InvoiceId == invoiceId)
            .OrderBy(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return [.. entities.Select(e => e.ToDto(dc))];
    }

    public async Task<bool> Cancel(Guid userId, Guid bookingId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.FoodBookings
            .Include(e => e.User)
            .Include(e => e.Food)
            .FirstOrDefaultAsync(e => e.Id == bookingId, cancellationToken);

        if (entity == null) return false;
        if (entity.UserId != userId) return false;
        if (entity.Food?.Type != (int)FoodType.Donation && entity.Status != (int)BookingStatus.Confirmed) return false;
        if (entity.Food?.Type == (int)FoodType.Donation && entity.Status != (int)BookingStatus.Completed) return false;

        if (entity.InvoiceId.HasValue) return false;

        entity.Status = (int)BookingStatus.Cancelled;
        entity.CancelledOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Complete(Guid bookingId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.FoodBookings.FindAsync([bookingId], cancellationToken);
        if (entity is null ||
            entity.Status != (int)BookingStatus.Confirmed ||
            entity.InvoiceId is not null)
        {
            return false;
        }

        entity.Status = (int)BookingStatus.Completed;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Create(Guid userId, Guid? foodId, int count, CancellationToken cancellationToken)
    {
        var food = await _dbContext.Foods.AsNoTracking().FirstOrDefaultAsync(e => e.Id == foodId, cancellationToken);
        if (food is null)
        {
            return false;
        }

        var entity = new Booking
        {
            Id = _dbContext.GeneratePk(),
            UserId = userId,
            FoodId = foodId,
            Status = (FoodType)food.Type == FoodType.Donation ? (int)BookingStatus.Completed : (int)BookingStatus.Confirmed,
            Count = count,
            BookedOn = DateTimeOffset.UtcNow
        };

        _dbContext.Add(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
