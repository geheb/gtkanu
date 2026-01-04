using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class Tryout
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public DateTimeOffset Date { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public int MaxBookings { get; set; }
    public DateTimeOffset? BookingStart { get; set; }
    public DateTimeOffset? BookingEnd { get; set; }
    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow > Date;

    internal ICollection<TryoutBooking>? TryoutBookings { get; set; }
    internal ICollection<TryoutChat>? TryoutChats { get; set; }

    internal TryoutDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Type = Type,
        Date = dc.ToLocal(Date),
        UserId = UserId!.Value,
        MaxBookings = MaxBookings,
        BookingStart = dc.ToLocal(BookingStart!.Value),
        BookingEnd = dc.ToLocal(BookingEnd!.Value),
        Description = Description,
    };

    internal void FromDto(TryoutDto dto)
    {
        Type = dto.Type;
        Date = dto.Date;
        UserId = dto.UserId;
        MaxBookings = dto.MaxBookings;
        BookingStart = dto.BookingStart;
        BookingEnd = dto.BookingEnd;
        Description = dto.Description?.Trim();
    }

    internal TryoutListDto ToListDto(int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        var canBook =
            DateTimeOffset.UtcNow >= BookingStart &&
            DateTimeOffset.UtcNow <= BookingEnd;

        return new()
        {
            Id = Id,
            Type = Type,
            Date = dc.ToLocal(Date),
            ContactName = User?.Name,
            ContactEmail = User?.EmailConfirmed == true ? User.Email : null,
            BookingCount = bookingCount,
            ChatMessageCount = chatMessageCount,
            BookingUsers = bookingUsers,
            MaxBookings = MaxBookings,
            IsExpired = Date < DateTimeOffset.UtcNow,
            Description = Description,
            CanAccept = canBook && bookingCount < MaxBookings,
        };
    }

    internal MyTryoutListDto ToMyListDto(TryoutBooking? booking, int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        var canBook =
            DateTimeOffset.UtcNow >= BookingStart &&
            DateTimeOffset.UtcNow <= BookingEnd;

        return new()
        {
            TryoutId = Id,
            Type = Type,
            BookingId = booking?.Id,
            Date = dc.ToLocal(Date),
            ContactName = User?.Name,
            ContactEmail = User?.EmailConfirmed == true ? User.Email : null,
            Description = Description,
            BookingName = booking?.Name ?? booking?.User?.Name ?? string.Empty,
            ChatMessageCount = chatMessageCount,
            BookingUsers = bookingUsers,
            BookingBookedOn = booking is null ? null : dc.ToLocal(booking.BookedOn),
            BookingConfirmedOn = booking?.ConfirmedOn is not null ? dc.ToLocal(booking.ConfirmedOn.Value) : null,
            BookingCancelledOn = booking?.CancelledOn is not null ? dc.ToLocal(booking.CancelledOn.Value) : null,
            CanAccept = canBook && bookingCount < MaxBookings,
            CanDelete = booking is not null && booking.ConfirmedOn is null,
            IsExpired = Date < DateTimeOffset.UtcNow,
        };
    }
}
