namespace GtKanu.Core.Models;

using GtKanu.Core.Converter;
using GtKanu.Core.Entities;
using System;

public class MyTripListDto
{
    public Guid? TripId { get; set; }
    public Guid? BookingId { get; set; }
    public string? TripTarget { get; set; }
    public DateTimeOffset TripStart { get; set; }
    public DateTimeOffset TripEnd { get; set; }
    public string? TripContactName { get; set; }
    public string? TripContactEmail { get; set; }
    public string? TripDescription { get; set; }
    public int BookingCount { get; }
    public int ChatMessageCount { get; }
    public int MaxBookings { get;  }
    public string? BookingName { get; set; }
    public string[] BookingUsers { get; set; } = Array.Empty<string>();

    public bool CanAccept { get; set; }
    public bool CanDelete { get; set; }
    public bool IsExpired { get; set; }
    public DateTimeOffset? BookingBookedOn { get; set; }
    public DateTimeOffset? BookingConfirmedOn { get; set; }
    public DateTimeOffset? BookingCancelledOn { get; set; }
    public string[] Categories { get; set; } = [];

    internal MyTripListDto(Trip trip, TripBooking? booking, int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        TripId = trip.Id;
        BookingId = booking?.Id;
        TripTarget = trip.Target;
        TripStart = dc.ToLocal(trip.Start);
        TripEnd = dc.ToLocal(trip.End);
        TripContactName = trip.User?.Name;
        TripContactEmail = trip.User?.EmailConfirmed == true ? trip.User.Email : null;
        TripDescription = trip.Description;
        MaxBookings = trip.MaxBookings;
        BookingName = booking?.Name ?? booking?.User?.Name ?? string.Empty;
        BookingUsers = bookingUsers;

        BookingCount = bookingCount;
        ChatMessageCount = chatMessageCount;

        var canBook =
            DateTimeOffset.UtcNow >= trip.BookingStart &&
            DateTimeOffset.UtcNow <= trip.BookingEnd;

        BookingBookedOn = booking is null ? null : dc.ToLocal(booking.BookedOn);

        BookingConfirmedOn = booking?.ConfirmedOn is not null ? dc.ToLocal(booking.ConfirmedOn.Value) : null;
        BookingCancelledOn = booking?.CancelledOn is not null ? dc.ToLocal(booking.CancelledOn.Value) : null;

        CanAccept = canBook && bookingCount < trip.MaxBookings;
        CanDelete = booking is not null && booking.ConfirmedOn is null;
        IsExpired = trip.IsExpired;

        var categories = (TripCategory)trip.Categories;
        if (categories != TripCategory.None)
        {
            var tripCategoryConverter = new TripCategoryConverter();
            Categories = Enum.GetValues<TripCategory>()
                .Where(v => v != TripCategory.None && categories.HasFlag(v))
                .Select(v => tripCategoryConverter.CategoryToName(v))
                .ToArray();
        }
    }
}
