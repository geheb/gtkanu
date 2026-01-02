using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace GtKanu.Core.Repositories;

public sealed class BookingFilter
{
    private const string DateFormat = "yyyy-MM-dd";
    private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("de-DE");
    public static readonly DateTime StartDate = new DateTime(2022, 5, 1);

    public IFormatProvider FormatProvider => _culture;

    public SelectListItem[] CreateListItems(DateTime select)
    {
        var items = new List<SelectListItem>();

        var date = StartDate;

        while (date <= DateTime.UtcNow.Date)
        {
            var name = date.ToString("Y", _culture);
            items.Add(new SelectListItem(name, date.ToString(DateFormat, CultureInfo.InvariantCulture), date == select));
            date = date.AddMonths(1);
        }
        items.Reverse();
        return items.ToArray();
    }

    public string ToDateFormatString(DateTime date)
    {
        return date.ToString(DateFormat, CultureInfo.InvariantCulture);
    }

    public DateTime ParseDateFirstOfMonth(string? value)
    {
        if (!DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var filterDate) ||
            filterDate < StartDate ||
            filterDate > DateTime.UtcNow ||
            filterDate.Day != 1)
        {
            filterDate = DateTime.UtcNow.AddDays(-DateTime.UtcNow.Day + 1);
        }

        return filterDate.Date;
    }

    public DateTime ParseDate(string? value)
    {
        if (!DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var filterDate) ||
            filterDate < StartDate ||
            filterDate > DateTime.UtcNow)
        {
            filterDate = DateTime.UtcNow;
        }

        return filterDate.Date;
    }
}
