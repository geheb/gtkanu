namespace GtKanu.Application.Models;

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
    public bool CanDelete { get; set; }
    public bool IsExpired { get; set; }
}
