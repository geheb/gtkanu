namespace GtKanu.Core.Entities;

public sealed class BoatRental
{
    public Guid Id { get; set; }
    public Guid? BoatId { get; set; }
    public Boat? Boat { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Purpose {  get; set; }
    public DateTimeOffset? CancelledOn { get; set; }
}
