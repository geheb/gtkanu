using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class CreateBoatRentalDto
{
    public Guid BoatId { get; set; }
    public Guid UserId { get; set; }
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }

    internal BoatRental ToEntity(Guid id)
    {
        return new()
        {
            Id = id,
            BoatId = BoatId,
            UserId = UserId,
            Start = Start,
            End = End,
            Purpose = Purpose
        };
    }
}
