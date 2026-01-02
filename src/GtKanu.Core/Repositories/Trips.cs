namespace GtKanu.Core.Repositories;

using GtKanu.Core.Converter;
using GtKanu.Core.Database;
using GtKanu.Core.Entities;
using GtKanu.Core.Extensions;
using GtKanu.Core.Models;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public sealed class Trips : IDisposable
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly AppDbContext _dbContext;
    private readonly SemaphoreSlim _bookingSemaphore = new SemaphoreSlim(1, 1);

    public Trips(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose() => _bookingSemaphore.Dispose();

    public async Task<bool> CreateTrip(TripDto dto, CancellationToken cancellationToken)
    {
        var entity = dto.ToEntity();
        entity.Id = _pkGenerator.Generate();

        await _dbContext.Set<Trip>().AddAsync(entity, cancellationToken);

        dto.Id = entity.Id;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<TripDto?> FindTrip(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<Trip>().FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return null;

        return new TripDto(entity, new GermanDateTimeConverter());
    }

    public async Task<TripBookingDto[]> GetBookingList(Guid tripId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Set<TripBooking>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.TripId == tripId)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new TripBookingDto(e, dc)).ToArray();
    }

    public async Task<TripListDto[]> GetTripList(bool showExpired, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var trips = await _dbContext.Set<Trip>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => (showExpired ? e.Start < now : e.End > now))
            .Select(e => new { trip = e, bookingCount = e.TripBookings!.Count, chatMessageCount = e.TripChats!.Count })
            .ToArrayAsync(cancellationToken);

        var tripIds = trips.Select(t => t.trip.Id).ToArray();
        var tripBookingUsers = new Dictionary<Guid, List<string>>();
        foreach (var ids in tripIds.Chunk(100))
        {
            var bookings = await _dbContext.Set<TripBooking>()
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => ids.Contains(e.TripId!.Value) && e.CancelledOn == null)
                .Select(e => new { TripId = e.TripId!.Value, e.Name, User = e.User!.Name! })
                .ToArrayAsync(cancellationToken);

            foreach (var b in bookings)
            {
                if (!tripBookingUsers.TryGetValue(b.TripId, out var users))
                {
                    users = new();
                    tripBookingUsers.Add(b.TripId, users);
                }
                users.Add(b.Name ?? b.User);
            }
        }

        var dc = new GermanDateTimeConverter();

        var result = trips.Select(e => new TripListDto(e.trip, e.bookingCount, e.chatMessageCount,
            tripBookingUsers.TryGetValue(e.trip.Id, out var u) ? u.ToArray() : Array.Empty<string>(), dc)).ToArray();

        return result.Where(r => r.Start >= now)
            .OrderBy(r => r.Start)
            .Concat(result.Where(r => r.Start < now).OrderByDescending(r => r.Start))
            .ToArray();
    }

    public async Task<TripListDto?> FindTripList(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<Trip>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.Id == id)
            .Select(e => new { trip = e, bookingCount = e.TripBookings!.Count, chatMessageCount = e.TripChats!.Count })
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null) return null;

        var bookings = await _dbContext.Set<TripBooking>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.TripId == id && e.CancelledOn == null)
            .Select(e => new { e.Name, User = e.User!.Name! })
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        var users = bookings.Select(b => b.Name ?? b.User).ToArray();

        return new TripListDto(entity.trip, entity.bookingCount, entity.chatMessageCount, users, dc);
    }

    public async Task<MyTripListDto[]> GetMyTripList(Guid userId, CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.Set<TripBooking>()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .ToArrayAsync(cancellationToken);

        var tripIds = bookings.Select(b => b.TripId!.Value).Distinct().ToArray();
        var tripBookingUsers = new Dictionary<Guid, List<string>>();
        foreach (var ids in tripIds.Chunk(100))
        {
            var bookingsSelected = await _dbContext.Set<TripBooking>()
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => ids.Contains(e.TripId!.Value) && e.CancelledOn == null)
                .Select(e => new { TripId = e.TripId!.Value, e.Name, User = e.User!.Name! })
                .ToArrayAsync(cancellationToken);

            foreach (var b in bookingsSelected)
            {
                if (!tripBookingUsers.TryGetValue(b.TripId, out var users))
                {
                    users = new();
                    tripBookingUsers.Add(b.TripId, users);
                }
                users.Add(b.Name ?? b.User);
            }
        }

        var dc = new GermanDateTimeConverter();
        var result = new List<MyTripListDto>();

        foreach (var batchBookings in bookings.Chunk(100))
        {
            tripIds = batchBookings.Select(e => e.TripId!.Value).Distinct().ToArray();

            var trips = await _dbContext.Set<Trip>()
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => tripIds.Contains(e.Id))
                .Select(e => new { trip = e, bookingCount = e.TripBookings!.Count, chatMessageCount = e.TripChats!.Count })
                .ToArrayAsync(cancellationToken);

            var tripMap = trips.ToDictionary(t => t.trip.Id);

            foreach (var b in batchBookings)
            {
                var t = tripMap[b.TripId!.Value];
                var users = tripBookingUsers.TryGetValue(b.TripId!.Value, out var u) ? u.ToArray() : Array.Empty<string>();
                result.Add(new MyTripListDto(t.trip, b, t.bookingCount, t.chatMessageCount, users, dc));
            }
        }

        var now = DateTimeOffset.UtcNow;
        return result.Where(r => r.TripStart >= now)
            .OrderBy(r => r.TripStart)
            .ThenByDescending(r => r.BookingBookedOn)
            .Concat(result.Where(r => r.TripStart < now).OrderByDescending(r => r.TripStart))
            .ToArray();
    }

    public async Task<TripBookingStatus> CreateBooking(Guid id, Guid userId, string? name, CancellationToken cancellationToken)
    {
        if (!await _bookingSemaphore.WaitAsync(TimeSpan.FromMinutes(1), cancellationToken)) return TripBookingStatus.Timeout;

        try
        {
            var trip = await _dbContext.Set<Trip>()
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new { e.MaxBookings, bookingCount = e.TripBookings!.Count })
                .FirstOrDefaultAsync(cancellationToken);

            if (trip == null || trip.bookingCount >= trip.MaxBookings) return TripBookingStatus.MaxReached;

            if (string.IsNullOrWhiteSpace(name)) name = null;

            var entity = await _dbContext.Set<TripBooking>()
                .FirstOrDefaultAsync(e => e.TripId == id && e.UserId == userId && e.Name == name, cancellationToken);

            if (entity != null)
            {
                return TripBookingStatus.AlreadyBooked;
            }

            entity = new()
            {
                Id = _pkGenerator.Generate(),
                BookedOn = DateTimeOffset.UtcNow,
                TripId = id,
                UserId = userId,
                Name = name
            };

            await _dbContext.Set<TripBooking>().AddAsync(entity, cancellationToken);

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? TripBookingStatus.Success : TripBookingStatus.Failed;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<bool> DeleteBooking(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<TripBooking>()
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
        var entity = await _dbContext.Set<TripBooking>()
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
        var entity = await _dbContext.Set<TripBooking>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        entity.ConfirmedOn = null;
        entity.CancelledOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateTrip(TripDto dto, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<Trip>().FindAsync(new object[] { dto.Id }, cancellationToken);
        if (entity == null) return false;

        var count = 0;
        if (entity.SetValue(e => e.Start, dto.Start)) count++;
        if (entity.SetValue(e => e.End, dto.End)) count++;
        if (entity.SetValue(e => e.UserId, dto.UserId)) count++;
        if (entity.SetValue(e => e.Target, dto.Target)) count++;
        if (entity.SetValue(e => e.MaxBookings, dto.MaxBookings)) count++;
        if (entity.SetValue(e => e.BookingStart, dto.BookingStart)) count++;
        if (entity.SetValue(e => e.BookingEnd, dto.BookingEnd)) count++;
        if (entity.SetValue(e => e.Description, dto.Description)) count++;
        if (entity.SetValue(e => e.Categories, (int)dto.Categories)) count++;
        if (entity.SetValue(e => e.IsPublic, dto.IsPublic)) count++;

        if (count < 1) return true;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<TripChatDto[]> GetChat(Guid tripId, Guid userId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Set<TripChat>()
            .Include(e => e.User)
            .Where(e => e.TripId == tripId)
            .OrderByDescending(e => e.CreatedOn)
            .ToArrayAsync(cancellationToken);

        if (entities.Length < 1) return Array.Empty<TripChatDto>();

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new TripChatDto(e, userId, dc)).ToArray();
    }

    public async Task<bool> CreateChatMessage(Guid id, Guid userId, string message, CancellationToken cancellationToken)
    {
        var trip = await _dbContext.Set<Trip>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (trip == null || trip.IsExpired) return false;

        var count = await _dbContext.Set<TripChat>().CountAsync(e => e.TripId == id, cancellationToken);
        if (count >= 10_000) return false;

        var entity = new TripChat
        {
            Id = _pkGenerator.Generate(),
            CreatedOn = DateTimeOffset.UtcNow,
            TripId = id,
            UserId = userId,
            Message = message
        };

        await _dbContext.Set<TripChat>().AddAsync(entity, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteTrip(Guid id, CancellationToken cancellationToken)
    {
        var trip = await _dbContext.Set<Trip>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (trip == null) return false;

        var bookings = await _dbContext.Set<TripBooking>().Where(e => e.TripId == id).ToArrayAsync(cancellationToken);
        if (bookings.Length > 0)
        {
            _dbContext.Set<TripBooking>().RemoveRange(bookings);
        }

        var chats = await _dbContext.Set<TripChat>().Where(e => e.TripId == id).ToArrayAsync(cancellationToken);
        if (chats.Length > 0)
        {
            _dbContext.Set<TripChat>().RemoveRange(chats);
        }

        _dbContext.Set<Trip>().Remove(trip);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<PublicTripDto[]> GetPublicTrips(CancellationToken cancellationToken)
    {
        var dbSetTrip = _dbContext.Set<Trip>();
        var now = DateTimeOffset.UtcNow;

        // Club regulation
        var start = new DateTimeOffset(new DateOnly(now.Year - 1, 10, 1), TimeOnly.MinValue, now.Offset);

        var trips = await dbSetTrip
            .AsNoTracking()
            .Where(e => e.Start >= start && e.IsPublic)
            .OrderBy(e => e.Start)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return trips
            .Select(e => new PublicTripDto(e, dc))
            .ToArray();
    }

    public async Task<string> GetPublicTripsAsIcs(CancellationToken cancellationToken)
    {
        var trips = await GetPublicTrips(cancellationToken);

        var calendar = new Ical.Net.Calendar();
        calendar.AddTimeZone(new VTimeZone("Europe/Berlin"));
        calendar.ProductId = "-//GT Kanu//NONSGML Fahrtenplan//EN";

        var tripCategoryConverter = new TripCategoryConverter();
        var dateTimeConverter = new GermanDateTimeConverter();

        foreach (var trip in trips)
        {
            var categories = string.Empty;
            if (trip.Categories != TripCategory.None)
            {
                var values = Enum.GetValues<TripCategory>()
                    .Where(e => e != TripCategory.None && trip.Categories.HasFlag(e))
                    .Select(e => tripCategoryConverter.CategoryToName(e));
                categories = string.Join(", ", values);
            }
            calendar.Events.Add(new()
            {
                Uid = trip.Id.ToString(),
                Summary = trip.Target,
                Description = $"Kategorien: {categories}",
                Start = new CalDateTime(dateTimeConverter.ToLocal(trip.Start).DateTime),
                End = new CalDateTime(dateTimeConverter.ToLocal(trip.End).DateTime),
            });
        }
        return new ComponentSerializer().SerializeToString(calendar)!;
    }
}
