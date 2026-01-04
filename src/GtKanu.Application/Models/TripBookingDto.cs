namespace GtKanu.Application.Models;

public sealed class TripBookingDto
{
    public Guid Id { get; set; }
    public string BookingUser { get; set; } = null!;
    public string? BookingPerson { get; set; }
    public DateTimeOffset BookedOn { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }
    public string BookingName => BookingPerson is not null ? (BookingPerson + $" (via {BookingUser})") : BookingUser;
}
