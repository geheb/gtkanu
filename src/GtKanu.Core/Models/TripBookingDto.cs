using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class TripBookingDto
{
    public Guid Id { get; set; }
    public string BookingUser { get; set; }
    public string? BookingPerson { get; set; }
    public DateTimeOffset BookedOn { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }

    public string BookingName => BookingPerson is not null ? (BookingPerson + $" (via {BookingUser})") : BookingUser;

    internal TripBookingDto(TripBooking entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        BookingUser = entity.User?.Name ?? string.Empty;
        BookingPerson = entity.Name;
        BookedOn = dc.ToLocal(entity.BookedOn);
        ConfirmedOn = entity.ConfirmedOn is not null ? dc.ToLocal(entity.ConfirmedOn.Value) : null;
        CancelledOn = entity.CancelledOn is not null ? dc.ToLocal(entity.CancelledOn.Value) : null;
    }
}
