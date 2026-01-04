using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class BoatRental
{
    public Guid Id { get; set; }
    public Guid? BoatId { get; set; }
    public Boat? Boat { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Purpose {  get; set; }
    public DateTimeOffset? CancelledOn { get; set; }

    public BoatRentalDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        BoatId = BoatId!.Value,
        UserId = UserId!.Value,
        UserEmail = User?.EmailConfirmed == true ? User.Email : null,
        User = User?.Name,
        Purpose = Purpose,
        Start = dc.ToLocal(Start),
        End = dc.ToLocal(End),
        IsFinished = End < DateTimeOffset.UtcNow,
        IsCancelled = CancelledOn is not null,
    };

    public MyBoatRentalListDto ToMyListDto(GermanDateTimeConverter dc) => new()
    {
        Purpose = Purpose,
        Start = dc.ToLocal(Start),
        End = dc.ToLocal(End),
        IsFinished = End < DateTimeOffset.UtcNow,
        IsCancelled = CancelledOn is not null
    };

    internal void FromDto(CreateBoatRentalDto dto)
    {
        BoatId = dto.BoatId;
        UserId = dto.UserId;
        Start = dto.Start;
        End = dto.End;
        Purpose = dto.Purpose?.Trim();
    }
}
