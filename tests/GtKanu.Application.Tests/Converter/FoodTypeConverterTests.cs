namespace GtKanu.Application.Tests.Converter;

using GtKanu.Application.Converter;
using GtKanu.Application.Models;

public class FoodTypeConverterTests
{
    private readonly FoodTypeConverter _sut = new();

    [Fact]
    public void TypeToString_Donation_ReturnsSpende()
    {
        var result = _sut.TypeToString(FoodType.Donation);
        result.Should().Be("Spende");
    }

    [Fact]
    public void TypeToString_Drink_ReturnsGetraenk()
    {
        var result = _sut.TypeToString(FoodType.Drink);
        result.Should().Be("Getränk");
    }

    [Fact]
    public void TypeToString_Dish_ReturnsSpeise()
    {
        var result = _sut.TypeToString(FoodType.Dish);
        result.Should().Be("Speise");
    }

    [Fact]
    public void TypeToString_Unknown_ReturnsUnknownText()
    {
        var result = _sut.TypeToString((FoodType)99);
        result.Should().Be("Unbekannt: 99");
    }
}
