namespace GtKanu.Application.Models;

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
    public int BookingCount { get; set; }
    public int ChatMessageCount { get; set; }
    public int MaxBookings { get; set; }
    public string? BookingName { get; set; }
    public string[] BookingUsers { get; set; } = [];
    public bool CanAccept { get; set; }
    public bool CanDelete { get; set; }
    public bool IsExpired { get; set; }
    public DateTimeOffset? BookingBookedOn { get; set; }
    public DateTimeOffset? BookingConfirmedOn { get; set; }
    public DateTimeOffset? BookingCancelledOn { get; set; }
    public string[] Categories { get; set; } = [];
}
