using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.VectorStores;

public abstract class BaseVectorStore : IVectorStore
{
    public abstract VectorStoreType StoreType { get; }
    public abstract string StoreName { get; }

    protected readonly Dictionary<string, InMemoryIndex> InMemoryIndices = new();

    public abstract Task CreateIndexAsync(string indexName, int dimensions, string? distanceMetric = "cosine", CancellationToken cancellationToken = default);
    public abstract Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
    public abstract Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);

    public abstract Task UpsertVectorsAsync(string indexName, List<EmbeddingVector> vectors, CancellationToken cancellationToken = default);
    public abstract Task DeleteVectorsAsync(string indexName, List<string> chunkIds, CancellationToken cancellationToken = default);
    public abstract Task DeleteVectorsByDocumentAsync(string indexName, string documentId, CancellationToken cancellationToken = default);

    public abstract Task<List<SearchResult>> SemanticSearchAsync(string indexName, float[] queryVector, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default);
    public abstract Task<List<SearchResult>> HybridSearchAsync(string indexName, float[] queryVector, string keywordQuery, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, double semanticWeight = 0.7, CancellationToken cancellationToken = default);
    public abstract Task<List<SearchResult>> KeywordSearchAsync(string indexName, string query, int topK = 10, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default);

    public abstract Task<long> GetVectorCountAsync(string indexName, CancellationToken cancellationToken = default);
    public abstract Task<Dictionary<string, object>> GetIndexStatsAsync(string indexName, CancellationToken cancellationToken = default);

    protected static double CosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length) return 0;

        double dotProduct = 0, mag1 = 0, mag2 = 0;
        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            mag1 += vector1[i] * vector1[i];
            mag2 += vector2[i] * vector2[i];
        }

        var magnitude = Math.Sqrt(mag1) * Math.Sqrt(mag2);
        return magnitude < double.Epsilon ? 0 : dotProduct / magnitude;
    }

    protected static List<SearchResult> ApplyScoreThreshold(List<(double Score, SearchResult Result)> scored, double? threshold)
    {
        return threshold.HasValue
            ? scored.Where(s => s.Score >= threshold.Value).OrderByDescending(s => s.Score).Select(s => s.Result).ToList()
            : scored.OrderByDescending(s => s.Score).Select(s => s.Result).ToList();
    }
}

public class InMemoryIndex
{
    public string Name { get; set; } = string.Empty;
    public int Dimensions { get; set; }
    public string DistanceMetric { get; set; } = "cosine";
    public List<IndexEntry> Entries { get; set; } = new();
}

public class IndexEntry
{
    public string Id { get; set; } = string.Empty;
    public string ChunkId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public float[] Vector { get; set; } = [];
    public string Content { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public DocumentFormat Format { get; set; }
    public int? PageNumber { get; set; }
    public string? Section { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
