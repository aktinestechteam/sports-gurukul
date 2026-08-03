using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Abstractions;

public record IndexMetrics(
    string IndexName,
    long DocumentsIndexed,
    long ChunksIndexed,
    long DocumentsFailed,
    DateTime? LastIndexedAt);

public record EmbeddingMetrics(
    long Calls,
    long TotalVectors,
    long Failures,
    double AverageMs,
    long TotalMs);

public record RetrievalMetrics(
    long Searches,
    long TotalResults,
    long TotalCandidates,
    double AverageLatencyMs,
    long TotalLatencyMs,
    long AccessDeniedCount);

public record SearchLatencyMetrics(
    long Requests,
    double P50Ms,
    double P95Ms,
    double AverageMs);

public record KnowledgeHealthReport(
    KnowledgeHealthState State,
    string? Message,
    IReadOnlyDictionary<string, KnowledgeComponentHealth> Components);

public record KnowledgeComponentHealth(string Component, bool Healthy, string? Detail = null);

public interface IKnowledgeMetricsCollector
{
    void RecordDocumentIndexed(string indexName, int chunkCount);
    void RecordDocumentFailed(string indexName, string? error);
    void RecordEmbedding(int batchSize, int dimensions, TimeSpan elapsed);
    void RecordEmbeddingFailure(string providerName);
    void RecordSearch(string indexName, SearchMode mode, int results, long elapsedMs, int candidates = 0);
    void RecordAccessDenied(string indexName);
    IndexMetrics GetIndexMetrics(string indexName);
    EmbeddingMetrics GetEmbeddingMetrics();
    RetrievalMetrics GetRetrievalMetrics();
    SearchLatencyMetrics GetSearchLatency();
}

public interface IKnowledgeHealthService
{
    Task<KnowledgeHealthReport> GetHealthAsync(CancellationToken ct = default);
}
