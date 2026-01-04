using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Trips;

public class TripInput
{
    [Display(Name = "Fahrt von")]
    [RequiredField]
    public string? Start { get; set; }

    [Display(Name = "Fahrt bis")]
    [RequiredField]
    public string? End { get; set; }

    [Display(Name = "Ziel")]
    [RequiredField, TextLengthField]
    public string? Target { get; set; }

    [Display(Name = "Ansprechpartner")]
    [RequiredField]
    public string? UserId { get; set; }

    [Display(Name = "Anmeldungen maximal")]
    [RequiredField, RangeField(1, 100)]
    public int MaxBookings { get; set; } = 20;

    [Display(Name = "Anmeldungen von")]
    [RequiredField]
    public string? BookingStart { get; set; }

    [Display(Name = "Anmeldungen bis")]
    [RequiredField]
    public string? BookingEnd { get; set; }

    [Display(Name = "Beschreibung")]
    [TextLengthField(8000)]
    public string? Description { get; set; }

    [Display(Name = "Kategorien")]
    public bool[] Categories { get; set; } = new bool[4];

    [Display(Name = "Ist öffentlich einsehbar")]
    public bool IsPublic { get; set; }

    public TripInput()
    {
        var dc = new GermanDateTimeConverter();
        var now = dc.ToLocal(DateTimeOffset.UtcNow);

        Start = dc.ToIso(now.AddDays(14));
        End = dc.ToIso(now.AddDays(15));
        BookingStart = dc.ToIso(now);
        BookingEnd = dc.ToIso(now.AddDays(13));
    }

    internal TripDto ToDto(Guid id = default)
    {
        var dc = new GermanDateTimeConverter();
        var categories = TripCategory.None;
        if (Categories[0]) categories |= TripCategory.Junior;
        if (Categories[1]) categories |= TripCategory.JuniorAdvanced;
        if (Categories[2]) categories |= TripCategory.Advanced;
        if (Categories[3]) categories |= TripCategory.YoungPeople;

        return new()
        {
            Id = id,
            Start = dc.FromIsoDateTime(Start)!.Value,
            End = dc.FromIsoDateTime(End)!.Value,
            Target = Target,
            UserId = Guid.Parse(UserId!),
            MaxBookings = MaxBookings,
            BookingStart = dc.FromIsoDateTime(BookingStart)!.Value,
            BookingEnd = dc.FromIsoDateTime(BookingEnd)!.Value,
            Description = Description,
            Categories = categories,
            IsPublic = IsPublic
        };
    }

    internal void From(TripDto dto)
    {
        var dc = new GermanDateTimeConverter();
        Start = dc.ToIso(dto.Start);
        End = dc.ToIso(dto.End);
        Target = dto.Target;
        UserId = dto.UserId.ToString();
        MaxBookings = dto.MaxBookings;
        BookingStart = dc.ToIso(dto.BookingStart);
        BookingEnd = dc.ToIso(dto.BookingEnd);
        Description = dto.Description;
        if (dto.Categories != TripCategory.None)
        {
            Categories[0] = dto.Categories.HasFlag(TripCategory.Junior);
            Categories[1] = dto.Categories.HasFlag(TripCategory.JuniorAdvanced);
            Categories[2] = dto.Categories.HasFlag(TripCategory.Advanced);
            Categories[3] = dto.Categories.HasFlag(TripCategory.YoungPeople);
        }
        IsPublic = dto.IsPublic;
    }

    public string? Validate()
    {
        var dc = new GermanDateTimeConverter();
        var startDate = dc.FromIsoDateTime(Start)!.Value;
        var endDate = dc.FromIsoDateTime(End)!.Value;

        if (startDate >= endDate)
        {
            return "Das Datum für die Fahrt ist ungültig.";
        }

        var regStartDate = dc.FromIsoDateTime(BookingStart)!.Value;
        var regEndDate = dc.FromIsoDateTime(BookingEnd)!.Value;

        if (regStartDate >= regEndDate || regStartDate >= startDate || regEndDate >= endDate)
        {
            return "Die Anmeldung sollte vor der Fahrt stattfinden.";
        }

        return null;
    }
}
