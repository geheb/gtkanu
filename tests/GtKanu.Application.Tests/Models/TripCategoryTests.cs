namespace GtKanu.Application.Tests.Models;

using FluentAssertions;
using GtKanu.Application.Models;

public class TripCategoryTests
{
    [Fact]
    public void Flags_ShouldBePowersOfTwo()
    {
        ((int)TripCategory.None).Should().Be(0);
        ((int)TripCategory.Junior).Should().Be(1);
        ((int)TripCategory.JuniorAdvanced).Should().Be(2);
        ((int)TripCategory.Advanced).Should().Be(4);
        ((int)TripCategory.YoungPeople).Should().Be(8);
    }

    [Fact]
    public void CombineFlags_ShouldWork()
    {
        var combined = TripCategory.Junior | TripCategory.Advanced;
        combined.HasFlag(TripCategory.Junior).Should().BeTrue();
        combined.HasFlag(TripCategory.Advanced).Should().BeTrue();
        combined.HasFlag(TripCategory.JuniorAdvanced).Should().BeFalse();
    }
}
