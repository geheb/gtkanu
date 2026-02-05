using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Hashing;
using System.Net;
using System.Text;

namespace GtKanu.Infrastructure.Security;

public sealed class BlacklistCache
{
    private static readonly string _prefix = Guid.NewGuid().ToString("N");

    private readonly IMemoryCache _cache;

    public BlacklistCache(IMemoryCache cache) => _cache = cache;

    public int Set(IPAddress address, string? name, int minScore)
    {
        var key = CreateKey(address, name);
        if (!_cache.TryGetValue(key, out int score))
        {
            score = 0;
        }

        if (score < minScore)
        {
            score = minScore;
        }
        else
        {
            score++;
        }

        _cache.Set(key, score, DateTimeOffset.UtcNow.AddHours(1));
        return score;
    }

    public int Get(IPAddress address, string? name = null) =>
        _cache.TryGetValue(CreateKey(address, name), out int score)
            ? score
            : 0;

    private static string CreateKey(IPAddress address, string? name)
    {
        var addr = address.GetAddressBytes();
        var id = name is null
            ? []
            : XxHash128.Hash(Encoding.UTF8.GetBytes(name));

        return _prefix + Convert.ToHexStringLower([.. addr, .. id]);
    }
}
