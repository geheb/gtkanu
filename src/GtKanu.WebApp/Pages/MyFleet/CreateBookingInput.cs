using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.MyFleet;

public class CreateBookingInput
{
    [Display(Name = "Fahrzeug")]
    [RequiredField]
    public string? VehicleId { get; set; }

    [Display(Name = "Zeitraum von")]
    [RequiredField]
    public string? Start { get; set; }

    [Display(Name = "Zeitraum bis")]
    [RequiredField]
    public string? End { get; set; }

    [Display(Name = "Anlass")]
    [RequiredField, TextLengthField(128)]
    public string? Purpose { get; set; }

    public CreateVehicleBookingDto ToDto(Guid userId)
    {
        var dc = new GermanDateTimeConverter();
        return new()
        {
            UserId = userId,
            VehicleId = Guid.Parse(VehicleId!),
            Start = dc.FromIsoDateTime(Start)!.Value,
            End = dc.FromIsoDateTime(End)!.Value,
            Purpose = Purpose
        };
    }
}
