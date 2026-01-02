using GtKanu.Core.Converter;
using GtKanu.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Models
{
    public sealed class InvoicePeriodDto
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string Period { get; set; }

        internal InvoicePeriodDto(InvoicePeriod entity, IFormatProvider formatProvider)
        {
            Id = entity.Id;
            Description = entity.Description;
            var from = entity.From;
            var to = entity.To;

            var isCurrentYear =
                from.Year == DateTime.UtcNow.Year &&
                to.Year == DateTime.UtcNow.Year;

            const string formatDate = "dd. MMMM yyyy";

            Period = isCurrentYear ?
                $"{from.ToString("dd. MMMM", formatProvider)} - {to.ToString(formatDate, formatProvider)}" :
                $"{from.ToString(formatDate, formatProvider)} - {to.ToString(formatDate, formatProvider)}";
        }
    }
}
