using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models
{
    public sealed class InvoiceDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string? User { get; set; }
        public decimal Total { get; set; }
        public decimal OpenTotal => Status == InvoiceStatus.Open ? Total : 0;
        public decimal PaidTotal => Status == InvoiceStatus.Paid ? Total : 0;
        public InvoiceStatus Status { get; set; }
        public DateTimeOffset? PaidOn { get; set; }
        public string? Description { get; set; }
        public string Period { get; set; }

        internal InvoiceDto(Invoice entity, GermanDateTimeConverter dc, IFormatProvider formatProvider)
        {
            Id = entity.Id;
            CreatedOn = dc.ToLocal(entity.CreatedOn);

            User = entity.User?.Name + $" ({entity.User?.DebtorNumber ?? "n.v."})";

            Total = entity.Total;
            Status = (InvoiceStatus)entity.Status;
            PaidOn = entity.PaidOn.HasValue ? dc.ToLocal(entity.PaidOn.Value) : default;

            var from = entity.InvoicePeriod!.From;
            var to = entity.InvoicePeriod!.To;
            var isCurrentYear = from.Year == DateTime.UtcNow.Year && to.Year == DateTime.UtcNow.Year;

            const string formatDate = "dd. MMMM yyyy";

            Period = isCurrentYear ?
                $"{from.ToString("dd. MMMM", formatProvider)} - {to.ToString(formatDate, formatProvider)}" :
                $"{from.ToString(formatDate, formatProvider)} - {to.ToString(formatDate, formatProvider)}";

            Description = entity.InvoicePeriod!.Description;
        }
    }
}
