namespace GtKanu.Application.Models;

using System;

public sealed class VehicleDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public bool IsInUse { get; set; }
}
