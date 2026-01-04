namespace GtKanu.Application.Models;

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
    public int ChatMessageCount { get; set; }
    public string[] BookingUsers { get; set; } = [];
}
