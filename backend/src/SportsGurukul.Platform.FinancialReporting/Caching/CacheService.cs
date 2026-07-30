using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Caching;

public class FinancialCacheService : IFinancialCacheService
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly ILogger<FinancialCacheService> _logger;

    public FinancialCacheService(ILogger<FinancialCacheService> logger)
    {
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
        {
            _logger.LogDebug("Cache hit for {Key}", key);
            return Task.FromResult(entry.Value as T);
        }

        if (entry is not null)
            _cache.TryRemove(key, out _);

        _logger.LogDebug("Cache miss for {Key}", key);
        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default) where T : class
    {
        var entry = new CacheEntry
        {
            Value = value,
            CreatedAt = DateTime.UtcNow,
            AbsoluteExpiration = options?.AbsoluteExpiration ?? TimeSpan.FromMinutes(5),
            SlidingExpiration = options?.SlidingExpiration ?? TimeSpan.FromMinutes(2)
        };

        _cache[key] = entry;
        _logger.LogDebug("Cached {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(key, out var entry) && !entry.IsExpired);
    }

    public string BuildKey(CacheRegion region, string identifier)
    {
        return $"fin:{region}:{identifier}";
    }

    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan AbsoluteExpiration { get; set; }
        public TimeSpan SlidingExpiration { get; set; }
        public bool IsExpired => DateTime.UtcNow - CreatedAt > AbsoluteExpiration;
    }
}
