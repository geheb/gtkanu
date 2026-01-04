namespace GtKanu.Application.Models;

public sealed class PublicTripDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Target { get; set; }
    public TripCategory Categories { get; set; }
}
