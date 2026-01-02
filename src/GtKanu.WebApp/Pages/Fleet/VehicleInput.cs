namespace GtKanu.WebApp.Pages.Fleet;

using GtKanu.WebApp.Annotations;
using System.ComponentModel.DataAnnotations;

public sealed class VehicleInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField(64)]
    public string? Name { get; set; }

    [Display(Name = "Kennzeichen")]
    [RegularExpression("^[A-ZÖÜÄ]{1,3}-[A-ZÖÜÄ]{1,2} \\d+$", ErrorMessage = "Das Feld 'Kennzeichen' muss in Großbuchstaben und folgendes Format haben: AB-CD 123")]
    [RequiredField, TextLengthField(12, MinimumLength = 3)]
    public string? Identifier { get; set; }

    [Display(Name = "Ist in Gebrauch")]
    public bool IsInUse { get; set; } = true;

    internal void From(VehicleDto dto)
    {
        Name = dto.Name;
        Identifier = dto.Identifier;
        IsInUse = dto.IsInUse;
    }

    internal void To(VehicleDto dto)
    {
        dto.Name = Name;
        dto.Identifier = Identifier;
        dto.IsInUse = IsInUse;
    }
}
