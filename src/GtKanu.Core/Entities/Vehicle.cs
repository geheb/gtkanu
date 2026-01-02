namespace GtKanu.Core.Entities;

using System;

internal sealed class Vehicle
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public bool IsInUse { get; set; }

    internal ICollection<VehicleBooking>? Bookings { get; set; }
}
