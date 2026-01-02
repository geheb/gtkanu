namespace GtKanu.Core.Models;

using GtKanu.Core.Converter;
using GtKanu.Core.Entities;
using System;

public sealed class VehicleBookingDto
{
    public Guid Id { get; set; }
    public string? VehicleName { get; set; }
    public string? VehicleIdentifier { get; set; }
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }
    public Guid? UserId { get; set; }
    public string? User { get; set; }
    public string? UserEmail { get; set; }
    public bool CanDelete { get; }
    public bool IsExpired { get; set; }

    internal VehicleBookingDto(VehicleBooking entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        VehicleName = entity.Vehicle?.Name;
        VehicleIdentifier = entity.Vehicle?.Identifier;
        Purpose = entity.Purpose;
        Start = dc.ToLocal(entity.Start);
        End = dc.ToLocal(entity.End);
        ConfirmedOn = entity.ConfirmedOn.HasValue ? dc.ToLocal(entity.ConfirmedOn.Value) : null;
        CancelledOn = entity.CancelledOn.HasValue ? dc.ToLocal(entity.CancelledOn.Value) : null;
        UserId = entity.UserId;
        User = entity.User?.Name;
        UserEmail = entity.User?.EmailConfirmed == true ? entity.User.Email : null;
        CanDelete = entity.ConfirmedOn is null;
        IsExpired = entity.IsExpired;
    }
}
