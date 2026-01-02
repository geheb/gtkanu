using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class TripListDto
{
    public Guid Id { get; set; }
    public string? Target { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public int BookingCount { get; }
    public int ChatMessageCount { get; }
    public int MaxBookings { get; }
    public bool IsExpired { get; }
    public string? Description { get; set; }
    public bool CanAccept { get; }
    public string[] BookingUsers { get; set; } = [];
    public string[] Categories { get; set; } = [];

    internal TripListDto(Trip trip, int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        Id = trip.Id;
        Target = trip.Target;
        Start = dc.ToLocal(trip.Start);
        End = dc.ToLocal(trip.End);
        ContactName = trip.User?.Name;
        ContactEmail = trip.User?.EmailConfirmed == true ? trip.User.Email : null;
        BookingCount = bookingCount;
        ChatMessageCount = chatMessageCount;

        MaxBookings = trip.MaxBookings;
        IsExpired = trip.IsExpired;
        Description = trip.Description;
        BookingUsers = bookingUsers;

        var canBook =
            DateTimeOffset.UtcNow >= trip.BookingStart &&
            DateTimeOffset.UtcNow <= trip.BookingEnd;

        CanAccept = canBook && bookingCount < trip.MaxBookings;

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
