using GtKanu.Application.Models;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Boats;

public sealed class BoatInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField(64)]
    public string? Name { get; set; }

    [Display(Name = "Nummer")]
    [RequiredField, TextLengthField(8, MinimumLength = 1)]
    public string? Identifier { get; set; }

    [Display(Name = "Standort")]
    [RequiredField, TextLengthField(64)]
    public string? Location { get; set; }

    [Display(Name = "Vermietung in Tagen maximal (0 = Langzeitmiete)")]
    [RequiredField, RangeField(0, 999)]
    public int MaxRentalDays { get; set; }

    [Display(Name = "Ist gesperrt")]
    public bool IsLocked { get; set; }

    [Display(Name = "Beschreibung")]
    [TextLengthField(1000)]
    public string? Description { get; set; }

    internal void From(BoatDto dto)
    {
        Name = dto.Name;
        Identifier = dto.Identifier;
        Location = dto.Location;
        MaxRentalDays = dto.MaxRentalDays;
        IsLocked = dto.IsLocked;
        Description = dto.Description;
    }
    
    internal BoatDto ToDto()
    {
        return new BoatDto()
        {
            Name = Name,
            Identifier = Identifier,
            Location = Location,
            MaxRentalDays = MaxRentalDays,
            IsLocked = IsLocked,
            Description = Description,
        };
    }
}
