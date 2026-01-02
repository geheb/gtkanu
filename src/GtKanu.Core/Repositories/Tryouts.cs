namespace GtKanu.Core.Repositories;

using GtKanu.Core.Converter;
using GtKanu.Core.Database;
using GtKanu.Core.Entities;
using GtKanu.Core.Extensions;
using GtKanu.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

public sealed class Tryouts
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly AppDbContext _dbContext;
    private readonly SemaphoreSlim _bookingSemaphore = new SemaphoreSlim(1, 1);

    public Tryouts(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose() => _bookingSemaphore.Dispose();

    public async Task<bool> CreateTryout(TryoutDto dto, CancellationToken cancellationToken)
    {
        var entity = dto.ToEntity();
        entity.Id = _pkGenerator.Generate();
        dto.Id = entity.Id;

        await _dbContext.Set<Tryout>().AddAsync(entity, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CreateTryout(TryoutDto[] dtos, CancellationToken cancellationToken)
    {
        foreach (var dto in dtos)
        {
            var entity = dto.ToEntity();
            entity.Id = _pkGenerator.Generate();
            dto.Id = entity.Id;

            await _dbContext.Set<Tryout>().AddAsync(entity, cancellationToken);
        }

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<TryoutDto?> FindTryout(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<Tryout>().FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return null;

        return new TryoutDto(entity, new GermanDateTimeConverter());
    }

    public async Task<TryoutBookingDto[]> GetBookingList(Guid tryoutId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Set<TryoutBooking>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.TryoutId == tryoutId)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new TryoutBookingDto(e, dc)).ToArray();
    }

    public async Task<TryoutListDto[]> GetTryoutList(bool showExpired, bool includeUserList, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var tryouts = await _dbContext.Set<Tryout>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => (showExpired ? e.Date < now : e.Date > now))
            .Select(e => new { tryout = e, bookingCount = e.TryoutBookings!.Count, chatMessageCount = e.TryoutChats!.Count })
            .ToArrayAsync(cancellationToken);

        if (tryouts.Length == 0)
        {
            return [];
        }

        Dictionary<Guid, string[]> usersByTryoutId;
        {
            var tryoutIds = tryouts.Select(t => t.tryout.Id).Distinct().ToArray();
            var bookingUsers = await _dbContext.Set<TryoutBooking>()
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => tryoutIds.Contains(e.TryoutId!.Value) && e.CancelledOn == null)
                .Select(e => new { tryoutId = e.TryoutId!.Value, e.Name, User = e.User!.Name })
                .ToArrayAsync(cancellationToken);

            usersByTryoutId = bookingUsers
                .GroupBy(e => e.tryoutId)
                .ToDictionary(e => e.Key, e => e.Select(i => i.Name ?? i.User!).ToArray());
        }

        var dc = new GermanDateTimeConverter();

        var result = tryouts
            .Select(e =>
            {
                if (!usersByTryoutId.TryGetValue(e.tryout.Id, out var users))
                {
                    users = [];
                }

                var bookingUsers = includeUserList ? users : [$"{users.Length}"];
                return new TryoutListDto(e.tryout, e.bookingCount, e.chatMessageCount, bookingUsers, dc);
            })
            .ToArray();

        return result.Where(r => r.Date >= now)
            .OrderBy(r => r.Date)
            .Concat(result.Where(r => r.Date < now).OrderByDescending(r => r.Date))
            .ToArray();
    }

    public async Task<bool> UpdateTryout(TryoutDto dto, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<Tryout>().FindAsync(new object[] { dto.Id }, cancellationToken);
        if (entity == null) return false;

        var count = 0;
        if (entity.SetValue(e => e.Type, dto.Type)) count++;
        if (entity.SetValue(e => e.Date, dto.Date)) count++;
        if (entity.SetValue(e => e.UserId, dto.UserId)) count++;
        if (entity.SetValue(e => e.MaxBookings, dto.MaxBookings)) count++;
        if (entity.SetValue(e => e.BookingStart, dto.BookingStart)) count++;
        if (entity.SetValue(e => e.BookingEnd, dto.BookingEnd)) count++;
        if (entity.SetValue(e => e.Description, dto.Description)) count++;

        if (count < 1) return true;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<TryoutBooking>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        entity.ConfirmedOn = DateTimeOffset.UtcNow;
        entity.CancelledOn = null;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CancelBooking(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<TryoutBooking>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        entity.ConfirmedOn = null;
        entity.CancelledOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteBooking(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<TryoutBooking>()
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);

        if (entity == null ||
            entity.ConfirmedOn is not null)
        {
            return false;
        }

        _dbContext.Remove(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<TryoutListDto?> FindTryoutList(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<Tryout>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.Id == id)
            .Select(e => new { tryout = e, bookingCount = e.TryoutBookings!.Count, chatMessageCount = e.TryoutChats!.Count })
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null) return null;

        var dc = new GermanDateTimeConverter();
        
        return new TryoutListDto(entity.tryout, entity.bookingCount, entity.chatMessageCount, [], dc);
    }

    public async Task<MyTryoutListDto[]> GetMyTryoutList(Guid userId, bool includeUserList, CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.Set<TryoutBooking>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        var result = new List<MyTryoutListDto>();

        foreach (var batchBookings in bookings.Chunk(100))
        {
            var ids = batchBookings.Select(e => e.TryoutId!.Value).Distinct().ToArray();

            var tryouts = await _dbContext.Set<Tryout>()
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => ids.Contains(e.Id))
                .Select(e => new { tryout = e, bookingCount = e.TryoutBookings!.Count, chatMessageCount = e.TryoutChats!.Count })
                .ToArrayAsync(cancellationToken);

            Dictionary<Guid, string[]> usersByTryoutId;
            {
                var bookingUsers = await _dbContext.Set<TryoutBooking>()
                    .AsNoTracking()
                    .Include(e => e.User)
                    .Where(e => ids.Contains(e.TryoutId!.Value) && e.CancelledOn == null)
                    .Select(e => new { tryoutId = e.TryoutId!.Value, e.Name, User = e.User!.Name })
                    .ToArrayAsync(cancellationToken);

                usersByTryoutId = bookingUsers
                    .GroupBy(e => e.tryoutId)
                    .ToDictionary(e => e.Key, e => e.Select(i => i.Name ?? i.User!).ToArray());
            }

            var map = tryouts.ToDictionary(t => t.tryout.Id);

            foreach (var b in batchBookings)
            {
                if (!usersByTryoutId.TryGetValue(b.TryoutId!.Value, out var users))
                {
                    users = [];
                }

                var bookingUsers = includeUserList ? users : [$"{users.Length}"];

                var t = map[b.TryoutId!.Value];
                result.Add(new MyTryoutListDto(t.tryout, b, t.bookingCount, t.chatMessageCount, bookingUsers, dc));
            }
        }

        var now = DateTimeOffset.UtcNow;
        return result.Where(r => r.Date >= now)
            .OrderBy(r => r.Date)
            .ThenByDescending(r => r.BookingBookedOn)
            .Concat(result.Where(r => r.Date < now).OrderByDescending(r => r.Date))
            .ToArray();
    }

    public async Task<TryoutBookingStatus> CreateBooking(Guid id, Guid userId, string? name, CancellationToken cancellationToken)
    {
        if (!await _bookingSemaphore.WaitAsync(TimeSpan.FromMinutes(1), cancellationToken)) return TryoutBookingStatus.Timeout;

        try
        {
            var tryout = await _dbContext.Set<Tryout>()
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new { e.MaxBookings, bookingCount = e.TryoutBookings!.Count })
                .FirstOrDefaultAsync(cancellationToken);

            if (tryout == null || tryout.bookingCount >= tryout.MaxBookings) return TryoutBookingStatus.MaxReached;

            if (string.IsNullOrWhiteSpace(name)) name = null;

            var entity = await _dbContext.Set<TryoutBooking>()
                .FirstOrDefaultAsync(e => e.TryoutId == id && e.UserId == userId && e.Name == name, cancellationToken);

            if (entity != null)
            {
                return TryoutBookingStatus.AlreadyBooked;
            }

            entity = new()
            {
                Id = _pkGenerator.Generate(),
                BookedOn = DateTimeOffset.UtcNow,
                TryoutId = id,
                UserId = userId,
                Name = name
            };

            await _dbContext.Set<TryoutBooking>().AddAsync(entity, cancellationToken);

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? TryoutBookingStatus.Success : TryoutBookingStatus.Failed;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<bool> DeleteTryout(Guid id, CancellationToken cancellationToken)
    {
        var tryout = await _dbContext.Set<Tryout>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (tryout == null) return false;

        var bookings = await _dbContext.Set<TryoutBooking>().Where(e => e.TryoutId == id).ToArrayAsync(cancellationToken);
        if (bookings.Length > 0)
        {
            _dbContext.Set<TryoutBooking>().RemoveRange(bookings);
        }

        _dbContext.Set<Tryout>().Remove(tryout);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<TryoutChatDto[]> GetChat(Guid tryoutId, Guid userId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Set<TryoutChat>()
            .Include(e => e.User)
            .Where(e => e.TryoutId == tryoutId)
            .OrderByDescending(e => e.CreatedOn)
            .ToArrayAsync(cancellationToken);

        if (entities.Length < 1) return [];

        var dc = new GermanDateTimeConverter();

        return [.. entities.Select(e => new TryoutChatDto(e, userId, dc))];
    }

    public async Task<bool> CreateChatMessage(Guid id, Guid userId, string message, CancellationToken cancellationToken)
    {
        var tryout = await _dbContext.Set<Tryout>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (tryout == null || tryout.IsExpired) return false;

        var count = await _dbContext.Set<TryoutChat>().CountAsync(e => e.TryoutId == id, cancellationToken);
        if (count >= 10_000) return false;

        var entity = new TryoutChat
        {
            Id = _pkGenerator.Generate(),
            CreatedOn = DateTimeOffset.UtcNow,
            TryoutId = id,
            UserId = userId,
            Message = message
        };

        await _dbContext.Set<TryoutChat>().AddAsync(entity, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
