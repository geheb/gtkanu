using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Models;

public sealed class BoatRentalListDto
{
    public Guid Id { get; set; }
    public BoatDto? Boat { get; set; }
    public int Count { get; set; }
}
