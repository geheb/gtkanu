namespace GtKanu.Application.Models;

public sealed class TryoutListDto
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public int BookingCount { get; set; }
    public int MaxBookings { get; set; }
    public bool IsExpired { get; set; }
    public string? Description { get; set; }
    public bool CanAccept { get; set; }
    public int ChatMessageCount { get; set; }
    public string[] BookingUsers { get; set; } = [];
}
