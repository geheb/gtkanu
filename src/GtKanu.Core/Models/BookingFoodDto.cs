using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models
{
    public sealed class BookingFoodDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset BookedOn { get; set; }
        public DateTimeOffset? CancelledOn { get; set; }
        public int Count { get; set; }
        public BookingStatus Status { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? User { get; set; }
        public InvoiceStatus? InvoiceStatus { get; set; }
        public DateTimeOffset? PaidOn { get; set; }
        public FoodType? Type { get; set; }

        public decimal Sum => Count * Price;

        public decimal Total =>
            Status != BookingStatus.Cancelled ?
            Sum : 0;

        public decimal OpenTotal =>
            Status != BookingStatus.Cancelled && 
            InvoiceStatus != Models.InvoiceStatus.Paid ?
            Total : 0;

        public bool IsCancelable =>
            InvoiceStatus is null &&
            ((Type != FoodType.Donation && Status == BookingStatus.Confirmed) ||
            (Type == FoodType.Donation && Status == BookingStatus.Completed));

        internal BookingFoodDto(Booking entity, GermanDateTimeConverter dc)
        {
            Id = entity.Id;
            BookedOn = dc.ToLocal(entity.BookedOn);
            CancelledOn = entity.CancelledOn.HasValue ? dc.ToLocal(entity.CancelledOn.Value) : default;
            Count = entity.Count;
            Status = (BookingStatus)entity.Status;
            Name = entity.Food?.Name;
            Price = entity.Food?.Price ?? 0;
            User = entity.User?.Name;
            InvoiceStatus = (InvoiceStatus?)entity.Invoice?.Status;
            PaidOn = entity.Invoice != null && entity.Invoice.PaidOn.HasValue ? dc.ToLocal(entity.Invoice.PaidOn.Value) : default;
            Type = (FoodType?)entity.Food?.Type;
        }
    }
}
