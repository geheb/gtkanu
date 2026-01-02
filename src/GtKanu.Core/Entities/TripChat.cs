namespace GtKanu.Core.Entities; 

using System;

internal sealed class TripChat
{
    public Guid Id { get; set; }
    public Guid? TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? Message { get; set; }
}
