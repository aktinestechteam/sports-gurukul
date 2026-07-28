using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Infrastructure.Caching;

public class SearchCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<SearchCacheService> _logger;
    private readonly ConcurrentDictionary<string, bool> _keyTracker = new();

    public SearchCacheService(IDistributedCache distributedCache, ILogger<SearchCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (json is null) return null;

            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get cached value for key {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            await _distributedCache.SetStringAsync(key, json, options, cancellationToken);
            _keyTracker.TryAdd(key, true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set cached value for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            _keyTracker.TryRemove(key, out _);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove cached value for key {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _keyTracker.Keys.Where(k => k.StartsWith(prefix)).ToList();
        foreach (var key in keysToRemove)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
                _keyTracker.TryRemove(key, out _);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove cached value for key {Key}", key);
            }
        }
    }
}
