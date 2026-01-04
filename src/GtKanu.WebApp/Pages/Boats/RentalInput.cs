using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Boats;

public sealed class RentalInput
{
    [Display(Name = "Mitglied")]
    [RequiredField]
    public string? UserId { get; set; }

    [Display(Name = "Zeitraum von")]
    [RequiredField]
    public string? Start { get; set; }

    [Display(Name = "Zeitraum bis")]
    [RequiredField]
    public string? End { get; set; }

    [Display(Name = "Anlass")]
    [RequiredField, TextLengthField(128)]
    public string? Purpose { get; set; }

    public string? Validate(int maxRentalDays)
    {
        var dc = new GermanDateTimeConverter();
        var localNow = dc.ToLocal(DateTimeOffset.UtcNow);
        var startDate = new DateTimeOffset(dc.FromIsoDate(Start)!.Value.ToDateTime(TimeOnly.MinValue), localNow.Offset);
        var endDate = new DateTimeOffset(dc.FromIsoDate(End)!.Value.ToDateTime(TimeOnly.MaxValue), localNow.Offset);

        if (startDate.Date < localNow.Date ||
            startDate >= endDate)
        {
            return "Das Datum für die Miete ist ungültig.";
        }

        if (maxRentalDays > 0 && (endDate - startDate).TotalDays > maxRentalDays)
        {
            return "Das Datum überschreitet die maximale Mietzeit.";
        }

        return null;
    }

    internal CreateBoatRentalDto ToDto(Guid boatId, Guid userId)
    {
        var dc = new GermanDateTimeConverter();

        var localOffset = dc.ToLocal(DateTimeOffset.UtcNow).Offset;
        var startDate = new DateTimeOffset(dc.FromIsoDate(Start)!.Value.ToDateTime(TimeOnly.MinValue), localOffset).ToUniversalTime();
        var endDate = new DateTimeOffset(dc.FromIsoDate(End)!.Value.ToDateTime(TimeOnly.MaxValue), localOffset).ToUniversalTime();

        return new()
        {
            BoatId = boatId,
            UserId = userId,
            Start = startDate,
            End = endDate,
            Purpose = Purpose,
        };
    }
}
