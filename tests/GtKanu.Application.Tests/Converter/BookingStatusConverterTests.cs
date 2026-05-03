namespace GtKanu.Application.Tests.Converter;

using FluentAssertions;
using GtKanu.Application.Converter;
using GtKanu.Application.Models;

public class BookingStatusConverterTests
{
    private readonly BookingStatusConverter _sut = new();

    [Fact]
    public void StatusToString_ConfirmedWithOpenInvoice_ReturnsInRechnungGestellt()
    {
        var booking = new BookingFoodDto
        {
            Status = BookingStatus.Confirmed,
            InvoiceStatus = InvoiceStatus.Open
        };
        var result = _sut.StatusToString(booking);
        result.Should().Be("In Rechnung gestellt");
    }

    [Fact]
    public void StatusToString_ConfirmedWithPaidInvoice_ReturnsPaidText()
    {
        var booking = new BookingFoodDto
        {
            Status = BookingStatus.Confirmed,
            InvoiceStatus = InvoiceStatus.Paid,
            PaidOn = DateTimeOffset.UtcNow
        };
        var result = _sut.StatusToString(booking);
        result.Should().StartWith("Als bezahlt markiert am");
    }

    [Fact]
    public void StatusToString_Cancelled_ReturnsCancelledText()
    {
        var booking = new BookingFoodDto
        {
            Status = BookingStatus.Cancelled,
            CancelledOn = DateTimeOffset.UtcNow
        };
        var result = _sut.StatusToString(booking);
        result.Should().StartWith("Storniert am");
    }

    [Fact]
    public void StatusToString_Completed_ReturnsAbgeschlossen()
    {
        var booking = new BookingFoodDto { Status = BookingStatus.Completed };
        var result = _sut.StatusToString(booking);
        result.Should().Be("Abgeschlossen");
    }

    [Fact]
    public void StatusToString_UnknownStatus_ReturnsUnknownText()
    {
        var booking = new BookingFoodDto { Status = (BookingStatus)99 };
        var result = _sut.StatusToString(booking);
        result.Should().StartWith("Unbekannt:");
    }

    [Fact]
    public void StatusToCssClass_Confirmed_ReturnsCartIcon()
    {
        var booking = new BookingFoodDto { Status = BookingStatus.Confirmed };
        var result = _sut.StatusToCssClass(booking);
        result.Should().Be("fas fa-cart-plus");
    }

    [Fact]
    public void StatusToCssClass_Cancelled_ReturnsThumbsDown()
    {
        var booking = new BookingFoodDto { Status = BookingStatus.Cancelled };
        var result = _sut.StatusToCssClass(booking);
        result.Should().Be("fas fa-thumbs-down has-text-danger");
    }

    [Fact]
    public void StatusToCssClass_Completed_ReturnsThumbsUp()
    {
        var booking = new BookingFoodDto { Status = BookingStatus.Completed };
        var result = _sut.StatusToCssClass(booking);
        result.Should().Be("fas fa-thumbs-up has-text-success");
    }

    [Fact]
    public void StatusToCssClass_Unknown_ReturnsEmpty()
    {
        var booking = new BookingFoodDto { Status = (BookingStatus)99 };
        var result = _sut.StatusToCssClass(booking);
        result.Should().BeEmpty();
    }
}
