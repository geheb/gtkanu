namespace GtKanu.Core.Entities;

internal class TripBooking
{
    public Guid Id { get; set; }
    public Guid? TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset BookedOn { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }
}
