using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Entities
{
    internal sealed class FoodList
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset ValidFrom { get; set; }
        public ICollection<Food>? Foods { get; set; }
    }
}
