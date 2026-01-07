using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class FoodInvoice
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public decimal Total { get; set; }
    public int Status { get; set; }
    public DateTimeOffset? PaidOn { get; set; }
    public ICollection<FoodBooking>? Bookings { get; set; }
    public Guid? InvoicePeriodId { get; set; }
    public FoodInvoicePeriod? InvoicePeriod { get; set; }

    internal InvoiceDto ToDto(GermanDateTimeConverter dc, IFormatProvider formatProvider)
    {
        var from = InvoicePeriod!.From;
        var to = InvoicePeriod!.To;
        var isCurrentYear = from.Year == DateTime.UtcNow.Year && to.Year == DateTime.UtcNow.Year;
        const string formatDate = "dd. MMMM yyyy";

        return new()
        {
            Id = Id,
            CreatedOn = dc.ToLocal(CreatedOn),
            User = User?.Name + $" ({User?.DebtorNumber ?? "n.v."})",
            Total = Total,
            Status = (InvoiceStatus)Status,
            PaidOn = PaidOn.HasValue ? dc.ToLocal(PaidOn.Value) : default,
            Period = isCurrentYear ?
            $"{from.ToString("dd. MMMM", formatProvider)} - {to.ToString(formatDate, formatProvider)}" :
            $"{from.ToString(formatDate, formatProvider)} - {to.ToString(formatDate, formatProvider)}",
            Description = InvoicePeriod!.Description,
        };
    }
}
