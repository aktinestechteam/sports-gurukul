using System.Collections.Concurrent;
using System.Diagnostics;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Observability;

internal sealed class KnowledgeMetricsCollector : IKnowledgeMetricsCollector
{
    private readonly int _latencySampleLimit;

    private readonly ConcurrentDictionary<string, IndexState> _indexes = new(StringComparer.Ordinal);

    private long _embeddingCalls;
    private long _embeddingVectors;
    private long _embeddingFailures;
    private long _embeddingTotalMs;

    private long _searches;
    private long _totalResults;
    private long _totalCandidates;
    private long _retrievalTotalMs;
    private long _accessDenied;

    private readonly object _latencySync = new();
    private readonly List<long> _latencySamples = new();

    public KnowledgeMetricsCollector(ObservabilityOptions options)
    {
        _latencySampleLimit = Math.Max(10, options.LatencySampleLimit);
    }

    public void RecordDocumentIndexed(string indexName, int chunkCount)
    {
        var state = _indexes.GetOrAdd(indexName, _ => new IndexState());
        state.DocumentsIndexed++;
        state.ChunksIndexed += chunkCount;
        state.LastIndexedAt = DateTime.UtcNow;
    }

    public void RecordDocumentFailed(string indexName, string? error)
    {
        var state = _indexes.GetOrAdd(indexName, _ => new IndexState());
        state.DocumentsFailed++;
    }

    public void RecordEmbedding(int batchSize, int dimensions, TimeSpan elapsed)
    {
        Interlocked.Increment(ref _embeddingCalls);
        Interlocked.Add(ref _embeddingVectors, batchSize);
        Interlocked.Add(ref _embeddingTotalMs, (long)elapsed.TotalMilliseconds);
    }

    public void RecordEmbeddingFailure(string providerName) => Interlocked.Increment(ref _embeddingFailures);

    public void RecordSearch(string indexName, SearchMode mode, int results, long elapsedMs, int candidates = 0)
    {
        Interlocked.Increment(ref _searches);
        Interlocked.Add(ref _totalResults, results);
        Interlocked.Add(ref _totalCandidates, candidates);
        Interlocked.Add(ref _retrievalTotalMs, elapsedMs);
        lock (_latencySync)
        {
            _latencySamples.Add(elapsedMs);
            if (_latencySamples.Count > _latencySampleLimit)
            {
                _latencySamples.RemoveRange(0, _latencySamples.Count - _latencySampleLimit);
            }
        }
    }

    public void RecordAccessDenied(string indexName) => Interlocked.Increment(ref _accessDenied);

    public IndexMetrics GetIndexMetrics(string indexName)
    {
        if (!_indexes.TryGetValue(indexName, out var state))
        {
            return new IndexMetrics(indexName, 0, 0, 0, null);
        }

        return new IndexMetrics(
            indexName,
            state.DocumentsIndexed,
            state.ChunksIndexed,
            state.DocumentsFailed,
            state.LastIndexedAt);
    }

    public EmbeddingMetrics GetEmbeddingMetrics()
    {
        var calls = Interlocked.Read(ref _embeddingCalls);
        var totalMs = Interlocked.Read(ref _embeddingTotalMs);
        return new EmbeddingMetrics(
            calls,
            Interlocked.Read(ref _embeddingVectors),
            Interlocked.Read(ref _embeddingFailures),
            calls == 0 ? 0 : totalMs / (double)calls,
            totalMs);
    }

    public RetrievalMetrics GetRetrievalMetrics()
    {
        var searches = Interlocked.Read(ref _searches);
        var totalMs = Interlocked.Read(ref _retrievalTotalMs);
        return new RetrievalMetrics(
            searches,
            Interlocked.Read(ref _totalResults),
            Interlocked.Read(ref _totalCandidates),
            searches == 0 ? 0 : totalMs / (double)searches,
            totalMs,
            Interlocked.Read(ref _accessDenied));
    }

    public SearchLatencyMetrics GetSearchLatency()
    {
        long[] samples;
        lock (_latencySync)
        {
            samples = _latencySamples.OrderBy(v => v).ToArray();
        }

        if (samples.Length == 0)
        {
            return new SearchLatencyMetrics(0, 0, 0, 0);
        }

        return new SearchLatencyMetrics(
            samples.Length,
            Percentile(samples, 0.50),
            Percentile(samples, 0.95),
            samples.Average());
    }

    private static double Percentile(IReadOnlyList<long> sorted, double percentile)
    {
        var count = sorted.Count;
        if (count == 0)
        {
            return 0;
        }

        if (count == 1)
        {
            return sorted[0];
        }

        if (percentile == 0.50 && count % 2 == 0)
        {
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }

        var index = (int)Math.Ceiling(percentile * count) - 1;
        return sorted[Math.Clamp(index, 0, count - 1)];
    }

    private sealed class IndexState
    {
        public long DocumentsIndexed;
        public long ChunksIndexed;
        public long DocumentsFailed;
        public DateTime? LastIndexedAt;
    }
}
