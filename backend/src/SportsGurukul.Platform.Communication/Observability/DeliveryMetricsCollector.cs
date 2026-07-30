using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.Communication.Observability;

public class DeliveryMetricsCollector
{
    private readonly ILogger<DeliveryMetricsCollector> _logger;
    private readonly ConcurrentDictionary<string, ChannelMetrics> _channelMetrics = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentQueue<DeliveryMetricPoint> _recentPoints = new();

    public DeliveryMetricsCollector(ILogger<DeliveryMetricsCollector> logger)
    {
        _logger = logger;
    }

    public void RecordDelivery(string channel, bool success, long durationMs)
    {
        var metrics = _channelMetrics.GetOrAdd(channel, _ => new ChannelMetrics());

        lock (metrics)
        {
            metrics.TotalCount++;
            if (success) metrics.SuccessCount++;
            else metrics.FailureCount++;
            metrics.TotalDurationMs += durationMs;
            metrics.LastDeliveryAt = DateTime.UtcNow;

            if (durationMs > metrics.MaxDurationMs)
                metrics.MaxDurationMs = durationMs;

            if (metrics.MinDurationMs == 0 || durationMs < metrics.MinDurationMs)
                metrics.MinDurationMs = durationMs;
        }

        _recentPoints.Enqueue(new DeliveryMetricPoint
        {
            Channel = channel,
            IsSuccess = success,
            DurationMs = durationMs,
            Timestamp = DateTime.UtcNow
        });

        while (_recentPoints.Count > 1000)
            _recentPoints.TryDequeue(out _);
    }

    public void RecordRetry(string channel)
    {
        var metrics = _channelMetrics.GetOrAdd(channel, _ => new ChannelMetrics());

        lock (metrics)
        {
            metrics.RetryCount++;
        }
    }

    public void RecordQueueDepth(string channel, int depth)
    {
        var metrics = _channelMetrics.GetOrAdd(channel, _ => new ChannelMetrics());

        lock (metrics)
        {
            metrics.CurrentQueueDepth = depth;
        }
    }

    public ChannelMetricsSnapshot GetChannelMetrics(string channel)
    {
        if (_channelMetrics.TryGetValue(channel, out var metrics))
        {
            lock (metrics)
            {
                return new ChannelMetricsSnapshot
                {
                    Channel = channel,
                    TotalCount = metrics.TotalCount,
                    SuccessCount = metrics.SuccessCount,
                    FailureCount = metrics.FailureCount,
                    RetryCount = metrics.RetryCount,
                    AverageDurationMs = metrics.TotalCount > 0
                        ? metrics.TotalDurationMs / metrics.TotalCount
                        : 0,
                    MaxDurationMs = metrics.MaxDurationMs,
                    MinDurationMs = metrics.MinDurationMs,
                    CurrentQueueDepth = metrics.CurrentQueueDepth,
                    SuccessRate = metrics.TotalCount > 0
                        ? (double)metrics.SuccessCount / metrics.TotalCount * 100
                        : 100,
                    LastDeliveryAt = metrics.LastDeliveryAt
                };
            }
        }

        return new ChannelMetricsSnapshot { Channel = channel };
    }

    public IReadOnlyDictionary<string, ChannelMetricsSnapshot> GetAllChannelMetrics()
    {
        return _channelMetrics.Keys
            .Select(GetChannelMetrics)
            .ToDictionary(m => m.Channel, m => m);
    }

    public GlobalMetricsSnapshot GetGlobalMetrics()
    {
        var allMetrics = GetAllChannelMetrics();

        return new GlobalMetricsSnapshot
        {
            TotalDeliveries = allMetrics.Values.Sum(m => m.TotalCount),
            TotalSuccesses = allMetrics.Values.Sum(m => m.SuccessCount),
            TotalFailures = allMetrics.Values.Sum(m => m.FailureCount),
            TotalRetries = allMetrics.Values.Sum(m => m.RetryCount),
            OverallSuccessRate = allMetrics.Values.Sum(m => m.TotalCount) > 0
                ? (double)allMetrics.Values.Sum(m => m.SuccessCount) / allMetrics.Values.Sum(m => m.TotalCount) * 100
                : 100,
            Channels = allMetrics
        };
    }

    public void LogMetricsSummary()
    {
        var global = GetGlobalMetrics();
        _logger.LogInformation(
            "Delivery Metrics: {Total} total, {Success} success ({Rate:F1}%), {Fail} failed, {Retry} retries",
            global.TotalDeliveries, global.TotalSuccesses, global.OverallSuccessRate,
            global.TotalFailures, global.TotalRetries);
    }

    private class ChannelMetrics
    {
        public long TotalCount;
        public long SuccessCount;
        public long FailureCount;
        public long RetryCount;
        public long TotalDurationMs;
        public long MaxDurationMs;
        public long MinDurationMs;
        public int CurrentQueueDepth;
        public DateTime LastDeliveryAt;
    }

    private struct DeliveryMetricPoint
    {
        public string Channel;
        public bool IsSuccess;
        public long DurationMs;
        public DateTime Timestamp;
    }
}

public class ChannelMetricsSnapshot
{
    public string Channel { get; set; } = string.Empty;
    public long TotalCount { get; set; }
    public long SuccessCount { get; set; }
    public long FailureCount { get; set; }
    public long RetryCount { get; set; }
    public double AverageDurationMs { get; set; }
    public long MaxDurationMs { get; set; }
    public long MinDurationMs { get; set; }
    public int CurrentQueueDepth { get; set; }
    public double SuccessRate { get; set; }
    public DateTime? LastDeliveryAt { get; set; }
}

public class GlobalMetricsSnapshot
{
    public long TotalDeliveries { get; set; }
    public long TotalSuccesses { get; set; }
    public long TotalFailures { get; set; }
    public long TotalRetries { get; set; }
    public double OverallSuccessRate { get; set; }
    public IReadOnlyDictionary<string, ChannelMetricsSnapshot> Channels { get; set; }
        = new Dictionary<string, ChannelMetricsSnapshot>();
}
