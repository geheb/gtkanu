using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class PublicTripDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Target { get; set; }
    public TripCategory Categories { get; set; }

    internal PublicTripDto(Trip entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        Start = dc.ToLocal(entity.Start);
        End = dc.ToLocal(entity.End);
        Target = entity.Target;
        Categories = (TripCategory)entity.Categories;
    }
}
