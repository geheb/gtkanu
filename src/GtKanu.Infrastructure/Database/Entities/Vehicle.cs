namespace GtKanu.Infrastructure.Database.Entities;

using GtKanu.Application.Models;
using System;

internal sealed class Vehicle
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public bool IsInUse { get; set; }

    internal ICollection<VehicleBooking>? Bookings { get; set; }

    internal VehicleDto ToDto() => new()
    {
        Id = Id,
        Name = Name,
        Identifier = Identifier,
        IsInUse = IsInUse
    };

    internal void FromDto(VehicleDto dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Identifier = dto.Identifier;
        IsInUse = dto.IsInUse;
    }
}
