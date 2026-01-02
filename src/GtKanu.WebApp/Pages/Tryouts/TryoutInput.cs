using GtKanu.WebApp.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Tryouts;

public sealed class TryoutInput
{
    [Display(Name = "Trainingsart")]
    [RequiredField, TextLengthField(64, MinimumLength = 3)]
    public string? Type { get; set; }

    [Display(Name = "Trainingsdatum")]
    [RequiredField]
    public string? Date { get; set; }

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
    [TextLengthField(1000)]
    public string? Description { get; set; }

    public TryoutInput()
    {
        var dc = new GermanDateTimeConverter();
        Date = dc.ToIso(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));
        BookingStart = dc.ToIso(DateTime.UtcNow);
        BookingEnd = dc.ToIso(DateTime.UtcNow.AddDays(6));
    }

    public string? Validate()
    {
        var dc = new GermanDateTimeConverter();
        var startDate = dc.FromIsoDateTime(Date)!.Value;

        var regStartDate = dc.FromIsoDateTime(BookingStart)!.Value;
        var regEndDate = dc.FromIsoDateTime(BookingEnd)!.Value;

        if (regStartDate >= regEndDate || regStartDate >= startDate || regEndDate >= startDate)
        {
            return "Die Anmeldung sollte vor dem Training stattfinden.";
        }

        return null;
    }

    internal TryoutDto ToDto(Guid id = default)
    {
        var dc = new GermanDateTimeConverter();
        return new()
        {
            Id = id,
            Type = Type,
            Date = dc.FromIsoDateTime(Date)!.Value,
            UserId = Guid.Parse(UserId!),
            MaxBookings = MaxBookings,
            BookingStart = dc.FromIsoDateTime(BookingStart)!.Value,
            BookingEnd = dc.FromIsoDateTime(BookingEnd)!.Value,
            Description = Description
        };
    }

    internal void From(TryoutDto dto)
    {
        var dc = new GermanDateTimeConverter();
        Type = dto.Type;
        Date = dc.ToIso(dto.Date);
        UserId = dto.UserId.ToString();
        MaxBookings = dto.MaxBookings;
        BookingStart = dc.ToIso(dto.BookingStart);
        BookingEnd = dc.ToIso(dto.BookingEnd);
        Description = dto.Description;
    }
}
