namespace GtKanu.Application.Tests.Converter;

using FluentAssertions;
using GtKanu.Application.Converter;
using GtKanu.Application.Models;

public class InvoiceStatusConverterTests
{
    private readonly InvoiceStatusConverter _sut = new();

    [Fact]
    public void StatusToString_Open_ReturnsOffen()
    {
        var invoice = new InvoiceDto { Status = InvoiceStatus.Open };
        var result = _sut.StatusToString(invoice);
        result.Should().Be("Offen");
    }

    [Fact]
    public void StatusToString_Paid_ReturnsPaidText()
    {
        var invoice = new InvoiceDto
        {
            Status = InvoiceStatus.Paid,
            PaidOn = DateTimeOffset.UtcNow
        };
        var result = _sut.StatusToString(invoice);
        result.Should().StartWith("Als bezahlt markiert am");
    }

    [Fact]
    public void StatusToString_Unknown_ReturnsUnknownText()
    {
        var invoice = new InvoiceDto { Status = (InvoiceStatus)99 };
        var result = _sut.StatusToString(invoice);
        result.Should().StartWith("Unbekannt:");
    }
}
