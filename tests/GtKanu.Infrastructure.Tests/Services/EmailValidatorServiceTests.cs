namespace GtKanu.Infrastructure.Tests.Services;

using FluentAssertions;
using GtKanu.Application.Services;
using GtKanu.Infrastructure.Email;
using GtKanu.Infrastructure.Security;
using NSubstitute;

public class EmailValidatorServiceTests
{
    private readonly IIpReputationChecker _reputationChecker = Substitute.For<IIpReputationChecker>();
    private readonly IEmailValidatorService _sut;

    public EmailValidatorServiceTests()
    {
        _sut = new EmailValidatorService(_reputationChecker);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_NullOrEmptyEmail_ReturnsFalse(string? email)
    {
        var result = await _sut.Validate(email!, CancellationToken.None);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("test@gmail.com")]
    [InlineData("test@GMAIL.COM")]
    [InlineData("test@gmx.de")]
    [InlineData("test@web.de")]
    [InlineData("test@outlook.com")]
    [InlineData("test@t-online.de")]
    [InlineData("test@icloud.com")]
    public async Task Validate_KnownProvider_ReturnsTrue(string email)
    {
        var result = await _sut.Validate(email, CancellationToken.None);
        result.Should().BeTrue();
        await _reputationChecker.DidNotReceive().IsListedMx(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Validate_InvalidEmailWithoutAt_ReturnsFalse()
    {
        var result = await _sut.Validate("invalid-email", CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_UnknownDomainWithGoodReputation_ReturnsTrue()
    {
        _reputationChecker.IsListedMx("unknown-domain.example", Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.Validate("test@unknown-domain.example", CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_UnknownDomainWithBadReputation_ReturnsFalse()
    {
        _reputationChecker.IsListedMx("bad-domain.example", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.Validate("test@bad-domain.example", CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_UnicodeDomain_ShouldConvertToAscii()
    {
        _reputationChecker.IsListedMx("xn--mller-kva.example", Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.Validate("test@müller.example", CancellationToken.None);
        result.Should().BeTrue();
    }
}
