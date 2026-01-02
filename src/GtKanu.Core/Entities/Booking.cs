using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Entities
{
    internal sealed class Booking
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public IdentityUserGuid? User { get; set; }
        public Guid? FoodId { get; set; }
        public Food? Food { get; set; }
        public int Status { get; set; }
        public int Count { get; set; }
        public DateTimeOffset BookedOn { get; set; }
        public DateTimeOffset? CancelledOn { get; set; }
        public Guid? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
