using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class MyTryoutListDto
{
    public Guid? TryoutId { get; set; }
    public string? Type { get; set; }
    public Guid? BookingId { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? Description { get; set; }
    public string? BookingName { get; set; }
    public bool CanAccept { get; set; }
    public bool CanDelete { get; set; }
    public bool IsExpired { get; set; }
    public DateTimeOffset? BookingBookedOn { get; set; }
    public DateTimeOffset? BookingConfirmedOn { get; set; }
    public DateTimeOffset? BookingCancelledOn { get; set; }
    public int ChatMessageCount { get; }
    public string[] BookingUsers { get; }

    internal MyTryoutListDto(Tryout tryout, TryoutBooking? booking, int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        TryoutId = tryout.Id;
        Type = tryout.Type;
        BookingId = booking?.Id;
        Date = dc.ToLocal(tryout.Date);
        ContactName = tryout.User?.Name;
        ContactEmail = tryout.User?.EmailConfirmed == true ? tryout.User.Email : null;
        Description = tryout.Description;
        BookingName = booking?.Name ?? booking?.User?.Name ?? string.Empty;
        ChatMessageCount = chatMessageCount;
        BookingUsers = bookingUsers;

        var canBook =
            DateTimeOffset.UtcNow >= tryout.BookingStart &&
            DateTimeOffset.UtcNow <= tryout.BookingEnd;

        BookingBookedOn = booking is null ? null : dc.ToLocal(booking.BookedOn);

        BookingConfirmedOn = booking?.ConfirmedOn is not null ? dc.ToLocal(booking.ConfirmedOn.Value) : null;
        BookingCancelledOn = booking?.CancelledOn is not null ? dc.ToLocal(booking.CancelledOn.Value) : null;

        CanAccept = canBook && bookingCount < tryout.MaxBookings;
        CanDelete = booking is not null && booking.ConfirmedOn is null;
        IsExpired = tryout.Date < DateTimeOffset.UtcNow;
    }
}
