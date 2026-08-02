using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IVectorStore
{
    VectorStoreType StoreType { get; }
    string StoreName { get; }

    Task CreateIndexAsync(string indexName, int dimensions, string? distanceMetric = "cosine", CancellationToken cancellationToken = default);
    Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);

    Task UpsertVectorsAsync(string indexName, List<EmbeddingVector> vectors, CancellationToken cancellationToken = default);
    Task DeleteVectorsAsync(string indexName, List<string> chunkIds, CancellationToken cancellationToken = default);
    Task DeleteVectorsByDocumentAsync(string indexName, string documentId, CancellationToken cancellationToken = default);

    Task<List<SearchResult>> SemanticSearchAsync(string indexName, float[] queryVector, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> HybridSearchAsync(string indexName, float[] queryVector, string keywordQuery, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, double semanticWeight = 0.7, CancellationToken cancellationToken = default);
    Task<List<SearchResult>> KeywordSearchAsync(string indexName, string query, int topK = 10, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default);

    Task<long> GetVectorCountAsync(string indexName, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object>> GetIndexStatsAsync(string indexName, CancellationToken cancellationToken = default);
}

public interface IVectorStoreFactory
{
    IVectorStore GetStore(VectorStoreType type);
    IVectorStore GetStore(string storeName);
    bool SupportsStore(VectorStoreType type);
}
