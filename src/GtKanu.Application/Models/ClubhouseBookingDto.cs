namespace GtKanu.Application.Models;

public sealed class ClubhouseBookingDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsExpired { get; set; }
}
