namespace GtKanu.Application.Tests.Models;

using GtKanu.Application.Models;

public class BoatDtoTests
{
    [Fact]
    public void RentalDetails_LongTerm_ReturnsCorrectText()
    {
        var dto = new BoatDto { MaxRentalDays = 0 };
        dto.RentalDetails.Should().Be("Langzeitmiete");
    }

    [Fact]
    public void RentalDetails_ShortTerm_ReturnsCorrectText()
    {
        var dto = new BoatDto { MaxRentalDays = 3 };
        dto.RentalDetails.Should().Be("max. 3 Tag(e) mieten");
    }

    [Fact]
    public void NameDetails_ReturnsFormattedName()
    {
        var dto = new BoatDto { Name = "Kanu 1", Identifier = "K1" };
        dto.NameDetails.Should().Be("Kanu 1 #K1");
    }

    [Fact]
    public void FullDetails_ReturnsCombinedDetails()
    {
        var dto = new BoatDto { Name = "Kanu 1", Identifier = "K1", MaxRentalDays = 2 };
        dto.FullDetails.Should().Be("Kanu 1 #K1, max. 2 Tag(e) mieten");
    }
}
