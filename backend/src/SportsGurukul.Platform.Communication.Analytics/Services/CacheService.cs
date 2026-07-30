using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<CacheService> _logger;

    public double HitRate => TotalRequests > 0 ? (double)Hits / TotalRequests * 100 : 0;
    public long Hits { get; private set; }
    public long Misses { get; private set; }
    private long TotalRequests => Hits + Misses;

    public CacheService(ILogger<CacheService> logger)
    {
        _logger = logger;
        StartEvictionTask();
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
        {
            Hits++;
            entry.LastAccessed = DateTime.UtcNow;
            return Task.FromResult((T?)entry.Value);
        }

        Misses++;
        _cache.TryRemove(key, out _);
        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken ct = default)
    {
        var expiry = expiration ?? TimeSpan.FromMinutes(5);
        _cache[key] = new CacheEntry(value, expiry);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key, CancellationToken ct = default)
    {
        return Task.FromResult(_cache.TryRemove(key, out _));
    }

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
            return Task.FromResult(true);
        return Task.FromResult(false);
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration, CancellationToken ct = default)
    {
        var cached = await GetAsync<T>(key, ct);
        if (cached is not null)
            return cached;

        var value = await factory();
        if (value is not null)
            await SetAsync(key, value, expiration, ct);
        return value;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
    {
        var regex = new System.Text.RegularExpressions.Regex(
            "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", ".*") + "$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        var keysToRemove = _cache.Keys.Where(k => regex.IsMatch(k)).ToList();
        foreach (var key in keysToRemove)
            _cache.TryRemove(key, out _);

        _logger.LogDebug("Removed {Count} cache entries matching pattern {Pattern}", keysToRemove.Count, pattern);
        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken ct = default)
    {
        _cache.Clear();
        Hits = 0;
        Misses = 0;
        _logger.LogDebug("Cache cleared");
        return Task.CompletedTask;
    }

    public Task<long> IncrementAsync(string key, long value = 1, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(key, out var entry) && entry.Value is long longVal)
        {
            entry.Value = longVal + value;
            return Task.FromResult(longVal + value);
        }
        _cache[key] = new CacheEntry(value, null);
        return Task.FromResult(value);
    }

    public Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken ct = default)
    {
        var result = new Dictionary<string, T?>();
        foreach (var key in keys)
            result[key] = GetAsync<T>(key, ct).GetAwaiter().GetResult();
        return Task.FromResult(result);
    }

    public Task SetManyAsync<T>(Dictionary<string, T> items, TimeSpan? expiration, CancellationToken ct = default)
    {
        var expiry = expiration ?? TimeSpan.FromMinutes(5);
        foreach (var (key, value) in items)
            _cache[key] = new CacheEntry(value, expiry);
        return Task.CompletedTask;
    }

    private void StartEvictionTask()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                var now = DateTime.UtcNow;
                var expired = _cache.Where(kvp => kvp.Value.IsExpired).Select(kvp => kvp.Key).ToList();
                foreach (var key in expired)
                    _cache.TryRemove(key, out _);
                if (expired.Count > 0)
                    _logger.LogDebug("Evicted {Count} expired cache entries", expired.Count);
            }
        });
    }

    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime CreatedAt { get; }
        public DateTime? ExpiresAt { get; }
        public DateTime LastAccessed { get; set; }
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

        public CacheEntry(object? value, TimeSpan? ttl)
        {
            Value = value;
            CreatedAt = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;
            if (ttl.HasValue)
                ExpiresAt = CreatedAt.Add(ttl.Value);
        }
    }
}
