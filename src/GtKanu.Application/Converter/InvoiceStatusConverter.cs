using GtKanu.Application.Models;

namespace GtKanu.Application.Converter;

public sealed class InvoiceStatusConverter
{
    private readonly GermanDateTimeConverter _dateTimeConverter = new GermanDateTimeConverter();

    public string StatusToString(InvoiceDto invoice)
    {
        return invoice.Status switch
        {
            InvoiceStatus.Open => "Offen",
            InvoiceStatus.Paid => "Als bezahlt markiert am " + _dateTimeConverter.ToDateTime(invoice.PaidOn!.Value),
            _ => $"Unbekannt: {invoice.Status}"
        };
    }
}
