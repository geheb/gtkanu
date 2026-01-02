using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Entities
{
    internal class InvoicePeriod
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
    }
}
