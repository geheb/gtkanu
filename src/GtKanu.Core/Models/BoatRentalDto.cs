using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class BoatRentalDto
{
    public Guid Id { get; set; }
    public Guid BoatId { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? User { get; set; }
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool IsFinished { get; set; }
    public bool IsCancelled { get; set; }

    public BoatRentalDto()
    {
    }

    public BoatRentalDto(BoatRental entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        BoatId = entity.BoatId!.Value;
        UserId = entity.UserId!.Value;
        UserEmail = entity.User?.EmailConfirmed == true ? entity.User.Email : null;
        User = entity.User?.Name;
        Purpose = entity.Purpose;
        Start = dc.ToLocal(entity.Start);
        End = dc.ToLocal(entity.End);
        IsFinished = entity.End < DateTimeOffset.UtcNow;
        IsCancelled = entity.CancelledOn is not null;
    }   
}
