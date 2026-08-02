using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IKnowledgeManagementService
{
    Task<KnowledgeIndexInfo> CreateIndexAsync(string indexName, VectorStoreType storeType, int dimensions, CancellationToken cancellationToken = default);
    Task<bool> DeleteIndexAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default);
    Task<KnowledgeIndexInfo> RebuildIndexAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default);
    Task<KnowledgeIndexInfo> IncrementalIndexAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default);
    Task<bool> ArchiveIndexAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> RestoreIndexAsync(string indexName, CancellationToken cancellationToken = default);
    Task<KnowledgeIndexInfo> GetIndexInfoAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default);
    Task<List<KnowledgeIndexInfo>> ListIndexesAsync(CancellationToken cancellationToken = default);
    Task<bool> OptimizeIndexAsync(string indexName, CancellationToken cancellationToken = default);
}

public interface IKnowledgeAccessService
{
    Task<bool> CanAccessDocumentAsync(string userId, string documentId, CancellationToken cancellationToken = default);
    Task<bool> CanAccessKnowledgeBaseAsync(string userId, string knowledgeBaseId, CancellationToken cancellationToken = default);
    Task<KnowledgeAccessPolicy> GetAccessPolicyAsync(string knowledgeBaseId, CancellationToken cancellationToken = default);
    Task SetAccessPolicyAsync(string knowledgeBaseId, KnowledgeAccessPolicy policy, CancellationToken cancellationToken = default);
}

public interface IKnowledgeObservabilityService
{
    Task RecordIndexMetricAsync(string indexName, string metricName, double value, Dictionary<string, string>? tags = null, CancellationToken cancellationToken = default);
    Task RecordEmbeddingMetricAsync(string modelName, int tokenCount, long durationMs, CancellationToken cancellationToken = default);
    Task RecordRetrievalMetricAsync(string indexName, string queryType, long durationMs, int resultCount, CancellationToken cancellationToken = default);
    Task RecordSearchLatencyAsync(string indexName, long durationMs, bool isCacheHit, CancellationToken cancellationToken = default);
    Task RecordKnowledgeHealthAsync(string indexName, bool isHealthy, string? message = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, double>> GetIndexMetricsAsync(string indexName, CancellationToken cancellationToken = default);
    Task<Dictionary<string, double>> GetEmbeddingMetricsAsync(string modelName, CancellationToken cancellationToken = default);
    Task<Dictionary<string, double>> GetRetrievalMetricsAsync(string indexName, CancellationToken cancellationToken = default);
    Task<Dictionary<string, double>> GetSearchLatencyMetricsAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> IsKnowledgeHealthyAsync(string indexName, CancellationToken cancellationToken = default);
}
