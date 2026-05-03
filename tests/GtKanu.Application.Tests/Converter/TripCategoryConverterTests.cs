namespace GtKanu.Application.Tests.Converter;

using FluentAssertions;
using GtKanu.Application.Converter;
using GtKanu.Application.Models;

public class TripCategoryConverterTests
{
    private readonly TripCategoryConverter _sut = new();

    [Fact]
    public void CategoryToClass_Junior_ReturnsInfo()
    {
        var result = _sut.CategoryToClass(TripCategory.Junior);
        result.Should().Be("has-text-info");
    }

    [Fact]
    public void CategoryToClass_JuniorAdvanced_ReturnsSuccess()
    {
        var result = _sut.CategoryToClass(TripCategory.JuniorAdvanced);
        result.Should().Be("has-text-success");
    }

    [Fact]
    public void CategoryToClass_Advanced_ReturnsDanger()
    {
        var result = _sut.CategoryToClass(TripCategory.Advanced);
        result.Should().Be("has-text-danger");
    }

    [Fact]
    public void CategoryToClass_YoungPeople_ReturnsWarning()
    {
        var result = _sut.CategoryToClass(TripCategory.YoungPeople);
        result.Should().Be("has-text-warning");
    }

    [Fact]
    public void CategoryToClass_None_ReturnsEmpty()
    {
        var result = _sut.CategoryToClass(TripCategory.None);
        result.Should().BeEmpty();
    }

    [Fact]
    public void CategoryToName_Junior_ReturnsA()
    {
        var result = _sut.CategoryToName(TripCategory.Junior);
        result.Should().Be("A");
    }

    [Fact]
    public void CategoryToName_JuniorAdvanced_ReturnsFA()
    {
        var result = _sut.CategoryToName(TripCategory.JuniorAdvanced);
        result.Should().Be("FA");
    }

    [Fact]
    public void CategoryToName_Advanced_ReturnsF()
    {
        var result = _sut.CategoryToName(TripCategory.Advanced);
        result.Should().Be("F");
    }

    [Fact]
    public void CategoryToName_YoungPeople_ReturnsJ()
    {
        var result = _sut.CategoryToName(TripCategory.YoungPeople);
        result.Should().Be("J");
    }

    [Fact]
    public void CategoryToName_None_ReturnsEmpty()
    {
        var result = _sut.CategoryToName(TripCategory.None);
        result.Should().BeEmpty();
    }
}
