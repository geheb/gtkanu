namespace GtKanu.Application.Models;

public sealed class TripDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Target { get; set; }
    public Guid UserId { get; set; }
    public int MaxBookings { get; set; }
    public DateTimeOffset BookingStart { get; set; }
    public DateTimeOffset BookingEnd { get; set; }
    public string? Description { get; set; }
    public TripCategory Categories { get; set; }
    public bool IsPublic {  get; set; }
}
