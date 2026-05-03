namespace GtKanu.Application.Tests.Models;

using FluentAssertions;
using GtKanu.Application.Models;

public class UserTwoFactorTests
{
    [Fact]
    public void Record_ShouldHoldValues()
    {
        var userTwoFactor = new UserTwoFactor(true, "secret123", "otpauth://test");
        userTwoFactor.IsEnabled.Should().BeTrue();
        userTwoFactor.SecretKey.Should().Be("secret123");
        userTwoFactor.AuthUri.Should().Be("otpauth://test");
    }

    [Fact]
    public void Record_Equality_ShouldWork()
    {
        var a = new UserTwoFactor(false, "key", "uri");
        var b = new UserTwoFactor(false, "key", "uri");
        a.Should().Be(b);
    }
}
