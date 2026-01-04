using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class TripBooking
{
    public Guid Id { get; set; }
    public Guid? TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset BookedOn { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }

    internal TripBookingDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        BookingUser = User?.Name ?? string.Empty,
        BookingPerson = Name,
        BookedOn = dc.ToLocal(BookedOn),
        ConfirmedOn = ConfirmedOn is not null ? dc.ToLocal(ConfirmedOn.Value) : null,
        CancelledOn = CancelledOn is not null ? dc.ToLocal(CancelledOn.Value) : null,
    };
}
