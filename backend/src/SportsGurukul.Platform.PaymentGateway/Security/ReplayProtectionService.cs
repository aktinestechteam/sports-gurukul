using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.PaymentGateway.Security;

public class ReplayProtectionService
{
    private readonly ConcurrentDictionary<string, DateTime> _nonceStore = new();
    private readonly ILogger<ReplayProtectionService> _logger;
    private readonly TimeSpan _nonceTtl = TimeSpan.FromMinutes(10);
    private readonly int _maxTimestampDriftSeconds = 300;

    public ReplayProtectionService(ILogger<ReplayProtectionService> logger)
    {
        _logger = logger;
        StartCleanupTask();
    }

    public bool IsReplayAttack(string webhookId, string nonce, DateTime timestamp)
    {
        if (string.IsNullOrWhiteSpace(webhookId) && string.IsNullOrWhiteSpace(nonce))
            return false;

        var key = webhookId ?? nonce;

        if (Math.Abs((DateTime.UtcNow - timestamp).TotalSeconds) > _maxTimestampDriftSeconds)
        {
            _logger.LogWarning("Replay attack detected: timestamp drift exceeds limit for {Key}", key);
            return true;
        }

        if (!_nonceStore.TryAdd(key, timestamp))
        {
            _logger.LogWarning("Replay attack detected: duplicate nonce/webhookId {Key}", key);
            return true;
        }

        return false;
    }

    public bool ValidateNonce(string nonce, DateTime timestamp)
    {
        if (string.IsNullOrWhiteSpace(nonce))
            return false;

        if (Math.Abs((DateTime.UtcNow - timestamp).TotalSeconds) > _maxTimestampDriftSeconds)
            return false;

        return _nonceStore.TryAdd(nonce, timestamp);
    }

    public void EvictExpired()
    {
        var cutoff = DateTime.UtcNow.Add(-_nonceTtl);
        var expired = _nonceStore.Where(kvp => kvp.Value < cutoff).Select(kvp => kvp.Key).ToList();
        foreach (var key in expired)
        {
            _nonceStore.TryRemove(key, out _);
        }
        if (expired.Count > 0)
            _logger.LogInformation("Evicted {Count} expired replay protection records", expired.Count);
    }

    private void StartCleanupTask()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(15));
                EvictExpired();
            }
        });
    }
}
