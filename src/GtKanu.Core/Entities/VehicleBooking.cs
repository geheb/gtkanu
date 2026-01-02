namespace GtKanu.Core.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

internal sealed class VehicleBooking
{
    public Guid Id { get; set; }
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public DateTimeOffset BookedOn { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }
    public string? Purpose { get; set; }

    [NotMapped]
    public bool IsExpired => End < DateTimeOffset.UtcNow;
}
