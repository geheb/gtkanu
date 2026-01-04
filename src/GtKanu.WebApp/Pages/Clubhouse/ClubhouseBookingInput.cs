using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Clubhouse;

public class ClubhouseBookingInput
{
    [Display(Name = "Buchung von")]
    [RequiredField]
    public string? Start { get; set; }

    [Display(Name = "Buchung bis")]
    [RequiredField]
    public string? End { get; set; }

    [Display(Name = "Titel")]
    [RequiredField, TextLengthField(128, MinimumLength = 3)]
    public string? Title { get; set; }

    [Display(Name = "Beschreibung")]
    [TextLengthField(1000)]
    public string? Description { get; set; }

    internal void From(ClubhouseBookingDto dto)
    {
        var dc = new GermanDateTimeConverter();
        Start = dc.ToIso(dto.Start);
        End = dc.ToIso(dto.End);
        Title = dto.Title;
        Description = dto.Description;
    }

    internal ClubhouseBookingDto ToDto(Guid id = default)
    {
        var dc = new GermanDateTimeConverter();
        return new()
        {
            Id = id,
            Start = dc.FromIsoDateTime(Start)!.Value,
            End = dc.FromIsoDateTime(End)!.Value,
            Title = Title,
            Description = Description
        };
    }

    internal string? Validate()
    {
        var dc = new GermanDateTimeConverter();
        var startDate = dc.FromIsoDateTime(Start)!.Value;
        var endDate = dc.FromIsoDateTime(End)!.Value;

        if (startDate >= endDate)
        {
            return "Das Datum für die Buchung ist ungültig.";
        }

        return null;
    }
}
