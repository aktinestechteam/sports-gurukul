using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.PaymentGateway.Security;

public class IdempotencyService
{
    private readonly ConcurrentDictionary<string, IdempotencyRecord> _store = new();
    private readonly ILogger<IdempotencyService> _logger;
    private readonly TimeSpan _defaultTtl = TimeSpan.FromHours(24);

    public IdempotencyService(ILogger<IdempotencyService> logger)
    {
        _logger = logger;
        StartCleanupTask();
    }

    public bool TryGetResult(string key, out object? result)
    {
        if (_store.TryGetValue(key, out var record) && !record.IsExpired)
        {
            result = record.Result;
            return true;
        }
        result = null;
        return false;
    }

    public bool TrySetResult(string key, object result, TimeSpan? ttl = null)
    {
        var added = _store.TryAdd(key, new IdempotencyRecord
        {
            Result = result,
            CreatedAt = DateTime.UtcNow,
            Ttl = ttl ?? _defaultTtl
        });

        if (added)
            _logger.LogDebug("Idempotency record created for key {Key}", key);

        return added;
    }

    public bool IsDuplicate(string key)
    {
        return _store.ContainsKey(key) && !_store[key].IsExpired;
    }

    public void Evict(string key)
    {
        _store.TryRemove(key, out _);
    }

    public void EvictExpired()
    {
        var expired = _store.Where(kvp => kvp.Value.IsExpired).Select(kvp => kvp.Key).ToList();
        foreach (var key in expired)
        {
            _store.TryRemove(key, out _);
        }
        if (expired.Count > 0)
            _logger.LogInformation("Evicted {Count} expired idempotency records", expired.Count);
    }

    private void StartCleanupTask()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(30));
                EvictExpired();
            }
        });
    }

    private class IdempotencyRecord
    {
        public object? Result { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan Ttl { get; set; }
        public bool IsExpired => DateTime.UtcNow - CreatedAt > Ttl;
    }
}
