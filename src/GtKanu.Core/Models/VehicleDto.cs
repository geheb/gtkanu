namespace GtKanu.Core.Models;

using GtKanu.Core.Entities;
using System;

public sealed class VehicleDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public bool IsInUse { get; set; }

    public VehicleDto()
    {
    }

    internal VehicleDto(Vehicle entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Identifier = entity.Identifier;
        IsInUse = entity.IsInUse;
    }

    internal Vehicle ToEntity()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Identifier = Identifier,
            IsInUse = IsInUse
        };
    }
}
