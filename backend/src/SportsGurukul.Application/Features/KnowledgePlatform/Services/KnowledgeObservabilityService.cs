using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class KnowledgeObservabilityService : IKnowledgeObservabilityService
{
    private readonly ILogger<KnowledgeObservabilityService> _logger;
    private readonly Dictionary<string, List<MetricPoint>> _metrics = new();
    private readonly object _lock = new();

    public KnowledgeObservabilityService(ILogger<KnowledgeObservabilityService> logger)
    {
        _logger = logger;
    }

    public Task RecordIndexMetricAsync(string indexName, string metricName, double value, Dictionary<string, string>? tags = null, CancellationToken cancellationToken = default)
    {
        var key = $"index:{indexName}:{metricName}";
        RecordMetric(key, value, tags);
        return Task.CompletedTask;
    }

    public Task RecordEmbeddingMetricAsync(string modelName, int tokenCount, long durationMs, CancellationToken cancellationToken = default)
    {
        RecordMetric($"embedding:{modelName}:tokens", tokenCount);
        RecordMetric($"embedding:{modelName}:duration_ms", durationMs);
        return Task.CompletedTask;
    }

    public Task RecordRetrievalMetricAsync(string indexName, string queryType, long durationMs, int resultCount, CancellationToken cancellationToken = default)
    {
        RecordMetric($"retrieval:{indexName}:{queryType}:duration_ms", durationMs);
        RecordMetric($"retrieval:{indexName}:{queryType}:result_count", resultCount);
        return Task.CompletedTask;
    }

    public Task RecordSearchLatencyAsync(string indexName, long durationMs, bool isCacheHit, CancellationToken cancellationToken = default)
    {
        RecordMetric($"search:{indexName}:latency_ms", durationMs);
        RecordMetric($"search:{indexName}:cache_hit", isCacheHit ? 1 : 0);
        return Task.CompletedTask;
    }

    public Task RecordKnowledgeHealthAsync(string indexName, bool isHealthy, string? message = null, CancellationToken cancellationToken = default)
    {
        RecordMetric($"health:{indexName}:healthy", isHealthy ? 1 : 0);
        if (message != null)
            _logger.LogWarning("Knowledge health for {Index}: {Message}", indexName, message);
        return Task.CompletedTask;
    }

    public Task<Dictionary<string, double>> GetIndexMetricsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetMetricSummary($"index:{indexName}:"));
    }

    public Task<Dictionary<string, double>> GetEmbeddingMetricsAsync(string modelName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetMetricSummary($"embedding:{modelName}:"));
    }

    public Task<Dictionary<string, double>> GetRetrievalMetricsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetMetricSummary($"retrieval:{indexName}:"));
    }

    public Task<Dictionary<string, double>> GetSearchLatencyMetricsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetMetricSummary($"search:{indexName}:"));
    }

    public Task<bool> IsKnowledgeHealthyAsync(string indexName, CancellationToken cancellationToken = default)
    {
        var metrics = GetMetricSummary($"health:{indexName}:");
        var healthy = metrics.GetValueOrDefault("healthy", 1);
        return Task.FromResult(healthy > 0);
    }

    private void RecordMetric(string key, double value, Dictionary<string, string>? tags = null)
    {
        lock (_lock)
        {
            if (!_metrics.ContainsKey(key))
                _metrics[key] = new List<MetricPoint>();

            _metrics[key].Add(new MetricPoint(value, DateTime.UtcNow, tags));
        }
    }

    private Dictionary<string, double> GetMetricSummary(string prefix)
    {
        lock (_lock)
        {
            var result = new Dictionary<string, double>();
            var relevant = _metrics.Where(m => m.Key.StartsWith(prefix));

            foreach (var kvp in relevant)
            {
                var values = kvp.Value.Select(v => v.Value).ToList();
                if (values.Count == 0) continue;

                var metricName = kvp.Key[prefix.Length..];
                result[$"{metricName}.avg"] = values.Average();
                result[$"{metricName}.max"] = values.Max();
                result[$"{metricName}.min"] = values.Min();
                result[$"{metricName}.count"] = values.Count;
                result[$"{metricName}.last"] = values.Last();
            }

            return result;
        }
    }

    private record MetricPoint(double Value, DateTime Timestamp, Dictionary<string, string>? Tags);
}
