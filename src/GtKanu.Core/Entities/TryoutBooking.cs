namespace GtKanu.Core.Entities;

internal sealed class TryoutBooking
{
    public Guid Id { get; set; }
    public Guid? TryoutId { get; set; }
    public Tryout? Tryout { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset BookedOn { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }
}
