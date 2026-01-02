using GtKanu.WebApp.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Tryouts;

public sealed class RecurringInput
{
    [Display(Name = "Trainingsart")]
    [RequiredField, TextLengthField(64, MinimumLength = 3)]
    public string? Type { get; set; }

    [Display(Name = "Terminserie Start")]
    [RequiredField]
    public string? RecurringStart { get; set; }

    [Display(Name = "Terminserie Ende")]
    [RequiredField]
    public string? RecurringEnd { get; set; }

    [Display(Name = "Trainingszeit")]
    [RequiredField]
    public string? Time { get; set; }

    [Display(Name = "Wöchentliche Wiederholung")]
    public bool[] RecurringDays { get; set; } = new bool[7];

    [Display(Name = "Ansprechpartner")]
    [RequiredField]
    public string? UserId { get; set; }

    [Display(Name = "Anmeldungen maximal")]
    [RequiredField, RangeField(1, 100)]
    public int MaxBookings { get; set; } = 20;

    [Display(Name = "Anmeldungen von")]
    [RequiredField]
    public string? BookingStart { get; set; }

    [Display(Name = "Beschreibung")]
    [TextLengthField(1000)]
    public string? Description { get; set; }

    public RecurringInput()
    {
        var dc = new GermanDateTimeConverter();
        RecurringStart = dc.ToIso(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));
        RecurringEnd = dc.ToIso(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));
        Time = dc.ToIso(new TimeOnly(15, 0));
        BookingStart = dc.ToIso(DateTime.UtcNow);
    }

    public string? Validate()
    {
        var dc = new GermanDateTimeConverter();
        var startDate = dc.FromIsoDate(RecurringStart)!.Value;
        var endDate = dc.FromIsoDate(RecurringEnd)!.Value;
        if (startDate >= endDate)
        {
            return "Das Startdatum der Terminserie ist ungültig.";
        }

        if (startDate.Year != endDate.Year)
        {
            return "Die Terminserie sollte im gleichen Jahr liegen.";
        }

        var time = dc.FromIsoTime(Time)!.Value;
        var start = dc.ToUtc(new DateTime(startDate, time));
        var regStartDate = dc.FromIsoDateTime(BookingStart)!.Value;
        if (regStartDate >= start)
        {
            return "Die Anmeldung sollte vor dem Training stattfinden.";
        }

        var dayOfWeek = GetDayOfWeek();

        if (dayOfWeek.Count == 0)
        {
            return "Die wöchentliche Wiederholung wurde nicht angegeben.";
        }

        var hasDayOfWeek = false;
        while (startDate <= endDate && !hasDayOfWeek)
        {
            hasDayOfWeek = dayOfWeek.Contains(startDate.DayOfWeek);
            startDate = startDate.AddDays(1);
        }

        if (!hasDayOfWeek)
        {
            return "Die Terminserie enhält keinen angegebenen Wochentag.";
        }

        return null;
    }

    internal TryoutDto[] ToDtos()
    {
        var dc = new GermanDateTimeConverter();
        var startDate = dc.FromIsoDate(RecurringStart)!.Value;
        var endDate = dc.FromIsoDate(RecurringEnd)!.Value;
        var time = dc.FromIsoTime(Time)!.Value;
        var bookingStart = dc.FromIsoDateTime(BookingStart)!.Value;
        var dayOfWeek = GetDayOfWeek();
        var result = new List<TryoutDto>();

        while (startDate <= endDate)
        {
            if (dayOfWeek.Contains(startDate.DayOfWeek))
            {
                var start = dc.ToUtc(new DateTime(startDate, time));
                result.Add(new()
                {
                    Type = Type,
                    Date = start,
                    UserId = Guid.Parse(UserId!),
                    MaxBookings = MaxBookings,
                    BookingStart = bookingStart,
                    BookingEnd = start.AddHours(-8),
                    Description = Description
                });
            }
            startDate = startDate.AddDays(1);
        }

        return [.. result];
    }

    private List<DayOfWeek> GetDayOfWeek()
    {
        var result = new List<DayOfWeek>();
        if (RecurringDays[0]) result.Add(DayOfWeek.Monday);
        if (RecurringDays[1]) result.Add(DayOfWeek.Tuesday);
        if (RecurringDays[2]) result.Add(DayOfWeek.Wednesday);
        if (RecurringDays[3]) result.Add(DayOfWeek.Thursday);
        if (RecurringDays[4]) result.Add(DayOfWeek.Friday);
        if (RecurringDays[5]) result.Add(DayOfWeek.Saturday);
        if (RecurringDays[6]) result.Add(DayOfWeek.Sunday);
        return result;
    }
}
