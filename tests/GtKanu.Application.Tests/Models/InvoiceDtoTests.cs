namespace GtKanu.Application.Tests.Models;

using FluentAssertions;
using GtKanu.Application.Models;

public class InvoiceDtoTests
{
    [Fact]
    public void OpenTotal_OpenStatus_ShouldReturnTotal()
    {
        var dto = new InvoiceDto { Total = 100m, Status = InvoiceStatus.Open };
        dto.OpenTotal.Should().Be(100m);
    }

    [Fact]
    public void OpenTotal_PaidStatus_ShouldReturnZero()
    {
        var dto = new InvoiceDto { Total = 100m, Status = InvoiceStatus.Paid };
        dto.OpenTotal.Should().Be(0m);
    }

    [Fact]
    public void PaidTotal_PaidStatus_ShouldReturnTotal()
    {
        var dto = new InvoiceDto { Total = 100m, Status = InvoiceStatus.Paid };
        dto.PaidTotal.Should().Be(100m);
    }

    [Fact]
    public void PaidTotal_OpenStatus_ShouldReturnZero()
    {
        var dto = new InvoiceDto { Total = 100m, Status = InvoiceStatus.Open };
        dto.PaidTotal.Should().Be(0m);
    }
}
