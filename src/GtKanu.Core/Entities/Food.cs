using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Entities
{
    internal sealed class Food
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Type { get; set; }
        public Guid? FoodListId { get; set; }
        public FoodList? FoodList { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
