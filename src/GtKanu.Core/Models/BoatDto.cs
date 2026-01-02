using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class BoatDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public int MaxRentalDays { get; set; }
    public bool IsLocked { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string RentalDetails => MaxRentalDays == 0 ? $"Langzeitmiete" : $"max. {MaxRentalDays} Tag(e) mieten";
    public string NameDetails => $"{Name} #{Identifier}";
    public string FullDetails => NameDetails + ", " + RentalDetails;

    public BoatDto()
    {
    }

    internal BoatDto(Boat entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Identifier = entity.Identifier;
        MaxRentalDays = entity.MaxRentalDays;
        IsLocked = entity.IsLocked;
        Location = entity.Location;
        Description = entity.Description;
    }

    internal Boat ToEntity()
    {
        return new()
        {
            Id = Id,
            Name = Name?.Trim(),
            Identifier = Identifier?.Trim(),
            MaxRentalDays = MaxRentalDays,
            IsLocked = IsLocked,
            Location = Location?.Trim(),
            Description = Description?.Trim()
        };
    }
}
