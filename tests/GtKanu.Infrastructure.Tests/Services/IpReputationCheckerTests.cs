namespace GtKanu.Infrastructure.Tests.Services;

using GtKanu.Infrastructure.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

public class IpReputationCheckerTests
{
    private readonly IpReputationChecker _sut;

    public IpReputationCheckerTests()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        _sut = new IpReputationChecker(NullLogger<IpReputationChecker>.Instance, memoryCache);
    }

    [Fact]
    public async Task IsListed_LoopbackIPv4_ReturnsFalse()
    {
        var result = await _sut.IsListed(System.Net.IPAddress.Loopback);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsListed_LoopbackIPv6_ReturnsFalse()
    {
        var result = await _sut.IsListed(System.Net.IPAddress.IPv6Loopback);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsListedMx_EmptyDomain_ShouldHandleGracefully()
    {
        var result = await _sut.IsListedMx("nonexistent-domain-12345.example", CancellationToken.None);
        // Depends on DNS resolution, but should not throw
    }
}
