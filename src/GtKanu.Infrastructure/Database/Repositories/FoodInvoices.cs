using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class FoodInvoices : IFoodInvoices
{
    private readonly AppDbContext _dbContext;

    public FoodInvoices(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InvoicePeriodDto[]> GetPeriods(CancellationToken cancellationToken)
    {
        var entities = await _dbContext.FoodInvoicePeriods
            .AsNoTracking()
            .OrderByDescending(e => e.To)
            .ToArrayAsync(cancellationToken);

        var ci = CultureInfo.GetCultureInfo("de-DE");

        return [.. entities.Select(e => e.ToDto(ci))];
    }

    public async Task<InvoiceDto[]> GetByPeriod(Guid id, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.FoodInvoices
            .AsNoTracking()
            .Include(e => e.User)
            .Include(e => e.InvoicePeriod)
            .Where(e => e.InvoicePeriodId == id)
            .OrderBy(e => e.User!.DebtorNumber)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        var ci = CultureInfo.GetCultureInfo("de-DE");

        return [.. entities.Select(e => e.ToDto(dc, ci))];
    }

    public async Task<InvoiceDto[]> GetAll(Guid userId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.FoodInvoices
            .AsNoTracking()
            .Include(e => e.InvoicePeriod)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        var ci = CultureInfo.GetCultureInfo("de-DE");

        return [.. entities.Select(e => e.ToDto(dc, ci))];
    }

    public async Task<InvoiceDto?> Find(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.FoodInvoices
            .AsNoTracking()
            .Include(e => e.InvoicePeriod)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);

        if (entity == null) return null;

        var dc = new GermanDateTimeConverter();
        var ci = CultureInfo.GetCultureInfo("de-DE");

        return entity.ToDto(dc, ci);
    }

    public async Task<int> Create(DateTime start, DateTime end, string description, CancellationToken cancellationToken)
    {
        var statusCompleted = (int)BookingStatus.Completed;
        var dc = new GermanDateTimeConverter();
        var startParam = dc.ToUtc(start);
        var endParam = dc.ToUtc(end);

        var userIds = await _dbContext.FoodBookings
            .AsNoTracking()
            .Where(e => e.BookedOn >= startParam && e.BookedOn <= endParam && e.Status == statusCompleted && e.InvoiceId == null)
            .Select(e => e.UserId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        if (!userIds.Any()) return 0;

        using var trans = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var period = new InvoicePeriod
        {
            Id = _dbContext.GeneratePk(),
            Description = description,
            From = startParam,
            To = endParam
        };

        _dbContext.Add(period);
        if (await _dbContext.SaveChangesAsync(cancellationToken) < 1) return -1;

        var count = 0;

        foreach (var user in userIds)
        {
            var bookings = await _dbContext.FoodBookings
                .Include(e => e.Food)
                .Where(e => e.UserId == user && e.BookedOn >= startParam && e.BookedOn <= endParam && e.Status == statusCompleted && e.InvoiceId == null)
                .ToArrayAsync(cancellationToken);

            if (!bookings.Any()) continue;

            decimal total = bookings.Sum(b => b.Food!.Price * b.Count);

            var invoice = new Invoice
            {
                Id = _dbContext.GeneratePk(),
                CreatedOn = DateTimeOffset.UtcNow,
                Status = (int)InvoiceStatus.Open,
                UserId = user,
                Total = total,
                InvoicePeriodId = period.Id
            };

            _dbContext.Add(invoice);
            Array.ForEach(bookings, b => b.InvoiceId = invoice.Id);

            if (await _dbContext.SaveChangesAsync(cancellationToken) < 1) return -1;

            count++;
        }

        if (count > 0)
        {
            await trans.CommitAsync(cancellationToken);
        }

        return count;
    }

    public async Task<bool> UpdateStatusPaid(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.FoodInvoices.FindAsync([id], cancellationToken);
        if (entity == null) return false;
        var status = (InvoiceStatus)entity.Status;
        if (status == InvoiceStatus.Paid) return true;
        entity.Status = (int)InvoiceStatus.Paid;
        entity.PaidOn = DateTimeOffset.UtcNow;
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateStatusOpen(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.FoodInvoices.FindAsync([id], cancellationToken);
        if (entity == null) return false;
        var status = (InvoiceStatus)entity.Status;
        if (status == InvoiceStatus.Open) return true;
        entity.Status = (int)InvoiceStatus.Open;
        entity.PaidOn = null;
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateStatusPaidAll(Guid periodId, CancellationToken cancellationToken)
    {
        const int statusOpen = (int)InvoiceStatus.Open;

        var entitites = await _dbContext.FoodInvoices
            .Where(e => e.InvoicePeriodId == periodId && e.Status == statusOpen)
            .ToArrayAsync(cancellationToken);

        if (entitites.Length < 1) return false;

        foreach (var e in entitites)
        {
            e.Status = (int)InvoiceStatus.Paid;
            e.PaidOn = DateTimeOffset.UtcNow;
        }

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
