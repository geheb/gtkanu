using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

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

    internal BookingFoodDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        BookedOn = dc.ToLocal(BookedOn),
        CancelledOn = CancelledOn.HasValue ? dc.ToLocal(CancelledOn.Value) : default,
        Count = Count,
        Status = (BookingStatus)Status,
        Name = Food?.Name,
        Price = Food?.Price ?? 0,
        User = User?.Name,
        InvoiceStatus = (InvoiceStatus?)Invoice?.Status,
        PaidOn = Invoice != null && Invoice.PaidOn.HasValue ? dc.ToLocal(Invoice.PaidOn.Value) : default,
        Type = (FoodType?)Food?.Type,
    };
}
