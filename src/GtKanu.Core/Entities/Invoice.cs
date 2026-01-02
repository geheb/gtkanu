using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Entities
{
    internal sealed class Invoice
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public IdentityUserGuid? User { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public decimal Total { get; set; }
        public int Status { get; set; }
        public DateTimeOffset? PaidOn { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public Guid? InvoicePeriodId { get; set; }
        public InvoicePeriod? InvoicePeriod { get; set; }
    }
}
