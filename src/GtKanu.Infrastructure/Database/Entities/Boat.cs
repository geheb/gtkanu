using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class Boat
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public string? Location { get; set; }
    public int MaxRentalDays { get; set; }
    public bool IsLocked { get; set; }
    public string? Description { get; set; }

    internal ICollection<BoatRental>? BoatRentals { get; set; }

    internal BoatDto ToDto() => new()
    {
        Id = Id,
        Name = Name,
        Identifier = Identifier,
        MaxRentalDays = MaxRentalDays,
        IsLocked = IsLocked,
        Location = Location,
        Description = Description,
    };

    internal void FromDto(BoatDto dto)
    {
        Id = dto.Id;
        Name = dto.Name?.Trim();
        Identifier = dto.Identifier?.Trim();
        MaxRentalDays = dto.MaxRentalDays;
        IsLocked = dto.IsLocked;
        Location = dto.Location?.Trim();
        Description = dto.Description?.Trim();
    }
}
