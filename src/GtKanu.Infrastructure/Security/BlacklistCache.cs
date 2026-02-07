using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Common;
using System.IO.Hashing;
using System.Net;
using System.Text;

namespace GtKanu.Infrastructure.Security;

public sealed class BlacklistCache
{
    private static readonly string _prefix = Guid.NewGuid().ToString("N");

    private readonly IMemoryCache _cache;
    private readonly TimeProvider _timeProvider;

    public sealed class Item
    {
        public uint Count { get; set; }
        public DateTimeOffset LastCall { get; set; }
        public double AvgSeconds { get; set; }
        public bool IsSuspicious => Count >= 7 && AvgSeconds < 1.0;
        public bool IsCulprit => Count >= 13 && AvgSeconds < 1.0;
    }

    public BlacklistCache(IMemoryCache cache, TimeProvider timeProvider)
    {
        _cache = cache;
        _timeProvider = timeProvider;
    }

    public Item? Get(IPAddress address, string? name)
    {
        var key = CreateKey(address, name);
        if (!_cache.TryGetValue<Item>(key, out var item) || item is null)
        {
            return null;
        }
        return item;
    }

    public Item Update(IPAddress address, string? name, bool isHighScore)
    {
        var key = CreateKey(address, name);
        if (!_cache.TryGetValue<Item>(key, out var item) || item is null)
        {
            item = new Item();
        }

        var now = _timeProvider.GetUtcNow();

        if (item.Count++ == 0 || (now - item.LastCall).TotalSeconds > 6.0)
        {
            item.LastCall = now;
            item.AvgSeconds = 0.0;
        }
        else
        {
            if (isHighScore)
            {
                item.Count += 3;
            }
            var diff = (now - item.LastCall).TotalSeconds;

            item.LastCall = now;
            item.AvgSeconds = item.AvgSeconds > 0.0 ? (item.AvgSeconds + diff) / 2.0 : diff;
        }
        _cache.Set(key, item, DateTimeOffset.UtcNow.AddHours(1));
        return item;
    }

    private static string CreateKey(IPAddress address, string? name)
    {
        var addr = address.GetAddressBytes();
        var id = name is null
            ? []
            : XxHash128.Hash(Encoding.UTF8.GetBytes(name));

        return _prefix + Convert.ToBase64String([.. addr, .. id]);
    }
}
