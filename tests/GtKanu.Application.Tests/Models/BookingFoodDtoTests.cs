namespace GtKanu.Application.Tests.Models;

using FluentAssertions;
using GtKanu.Application.Models;

public class BookingFoodDtoTests
{
    [Fact]
    public void Sum_ShouldCalculateCorrectly()
    {
        var dto = new BookingFoodDto { Count = 3, Price = 5.50m };
        dto.Sum.Should().Be(16.50m);
    }

    [Fact]
    public void Total_Confirmed_ShouldReturnSum()
    {
        var dto = new BookingFoodDto { Count = 2, Price = 10m, Status = BookingStatus.Confirmed };
        dto.Total.Should().Be(20m);
    }

    [Fact]
    public void Total_Cancelled_ShouldReturnZero()
    {
        var dto = new BookingFoodDto { Count = 2, Price = 10m, Status = BookingStatus.Cancelled };
        dto.Total.Should().Be(0m);
    }

    [Fact]
    public void OpenTotal_ConfirmedAndOpen_ShouldReturnTotal()
    {
        var dto = new BookingFoodDto { Count = 2, Price = 10m, Status = BookingStatus.Confirmed, InvoiceStatus = InvoiceStatus.Open };
        dto.OpenTotal.Should().Be(20m);
    }

    [Fact]
    public void OpenTotal_Paid_ShouldReturnZero()
    {
        var dto = new BookingFoodDto { Count = 2, Price = 10m, Status = BookingStatus.Confirmed, InvoiceStatus = InvoiceStatus.Paid };
        dto.OpenTotal.Should().Be(0m);
    }

    [Fact]
    public void OpenTotal_Cancelled_ShouldReturnZero()
    {
        var dto = new BookingFoodDto { Count = 2, Price = 10m, Status = BookingStatus.Cancelled };
        dto.OpenTotal.Should().Be(0m);
    }

    [Fact]
    public void IsCancelable_NonDonationConfirmedWithoutInvoice_ShouldBeTrue()
    {
        var dto = new BookingFoodDto { Status = BookingStatus.Confirmed, Type = FoodType.Dish, InvoiceStatus = null };
        dto.IsCancelable.Should().BeTrue();
    }

    [Fact]
    public void IsCancelable_NonDonationConfirmedWithInvoice_ShouldBeFalse()
    {
        var dto = new BookingFoodDto { Status = BookingStatus.Confirmed, Type = FoodType.Dish, InvoiceStatus = InvoiceStatus.Open };
        dto.IsCancelable.Should().BeFalse();
    }

    [Fact]
    public void IsCancelable_DonationCompleted_ShouldBeTrue()
    {
        var dto = new BookingFoodDto { Status = BookingStatus.Completed, Type = FoodType.Donation };
        dto.IsCancelable.Should().BeTrue();
    }

    [Fact]
    public void IsCancelable_DonationConfirmed_ShouldBeFalse()
    {
        var dto = new BookingFoodDto { Status = BookingStatus.Confirmed, Type = FoodType.Donation };
        dto.IsCancelable.Should().BeFalse();
    }
}
