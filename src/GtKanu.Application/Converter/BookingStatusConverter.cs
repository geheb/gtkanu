using GtKanu.Application.Models;

namespace GtKanu.Application.Converter;

public sealed class BookingStatusConverter
{
    private readonly GermanDateTimeConverter _dateTimeConverter = new GermanDateTimeConverter();

    public string StatusToString(BookingFoodDto booking)
    {
        switch (booking.Status)
        {
            case BookingStatus.Confirmed:
                {
                    switch (booking.InvoiceStatus)
                    {
                        case InvoiceStatus.Open: return "In Rechnung gestellt";
                        case InvoiceStatus.Paid: return "Als bezahlt markiert am " + _dateTimeConverter.ToDateTime(booking.PaidOn!.Value);
                        default: return "Offen";
                    };
                }

            case BookingStatus.Cancelled: return "Storniert am " + _dateTimeConverter.ToDateTime(booking.CancelledOn!.Value);
            case BookingStatus.Completed: return "Abgeschlossen";
            default: return $"Unbekannt: {booking.Status}";
        };
    }

    public string StatusToCssClass(BookingFoodDto booking)
    {
        switch (booking.Status)
        {
            case BookingStatus.Confirmed: return "fas fa-cart-plus";
            case BookingStatus.Cancelled: return "fas fa-thumbs-down has-text-danger";
            case BookingStatus.Completed: return "fas fa-thumbs-up has-text-success";
            default: return string.Empty;
        };
    }
}
