namespace GtKanu.Application.Tests.Models;

using GtKanu.Application.Models;

public class RolesTests
{
    [Fact]
    public void IsMemberWithRole_MemberWithAdmin_ReturnsTrue()
    {
        var result = Roles.IsMemberWithRole([Roles.Member, Roles.Admin]);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMemberWithRole_MemberOnly_ReturnsFalse()
    {
        var result = Roles.IsMemberWithRole([Roles.Member]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMemberWithRole_MemberAndInterested_ReturnsFalse()
    {
        var result = Roles.IsMemberWithRole([Roles.Member, Roles.Interested]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMemberWithRole_NotMember_ReturnsFalse()
    {
        var result = Roles.IsMemberWithRole([Roles.Admin]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMemberWithRole_MemberWithTreasurer_ReturnsTrue()
    {
        var result = Roles.IsMemberWithRole([Roles.Member, Roles.Treasurer]);
        result.Should().BeTrue();
    }
}
