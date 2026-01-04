using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class InvoicePeriod
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public ICollection<Invoice>? Invoices { get; set; }


    internal InvoicePeriodDto ToDto(IFormatProvider formatProvider)
    {
        var isCurrentYear =
            From.Year == DateTime.UtcNow.Year &&
            To.Year == DateTime.UtcNow.Year;

        const string formatDate = "dd. MMMM yyyy";

        return new()
        {
            Id = Id,
            Description = Description,
            Period = isCurrentYear
                ? $"{From.ToString("dd. MMMM", formatProvider)} - {To.ToString(formatDate, formatProvider)}"
                : $"{From.ToString(formatDate, formatProvider)} - {To.ToString(formatDate, formatProvider)}",
        };
    }
}
