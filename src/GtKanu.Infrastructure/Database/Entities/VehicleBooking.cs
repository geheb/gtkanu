namespace GtKanu.Infrastructure.Database.Entities;

using GtKanu.Application.Converter;
using GtKanu.Application.Models;
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

    internal VehicleBookingDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        VehicleName = Vehicle?.Name,
        VehicleIdentifier = Vehicle?.Identifier,
        Purpose = Purpose,
        Start = dc.ToLocal(Start),
        End = dc.ToLocal(End),
        ConfirmedOn = ConfirmedOn.HasValue ? dc.ToLocal(ConfirmedOn.Value) : null,
        CancelledOn = CancelledOn.HasValue ? dc.ToLocal(CancelledOn.Value) : null,
        UserId = UserId,
        User = User?.Name,
        UserEmail = User?.EmailConfirmed == true ? User.Email : null,
        CanDelete = ConfirmedOn is null,
        IsExpired = IsExpired,
    };
}
