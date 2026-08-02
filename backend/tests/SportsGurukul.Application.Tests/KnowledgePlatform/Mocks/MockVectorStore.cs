using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;
using ModelsCitation = SportsGurukul.Application.Features.KnowledgePlatform.Models.Citation;

namespace SportsGurukul.Application.Tests.KnowledgePlatform.Mocks;

public class MockVectorStore : IVectorStore
{
    public VectorStoreType StoreType => VectorStoreType.Qdrant;
    public string StoreName => "MockStore";

    private readonly List<(string IndexName, IndexEntry Entry)> _store = new();

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

    public Task CreateIndexAsync(string indexName, int dimensions, string? distanceMetric = "cosine", CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        _store.RemoveAll(s => s.IndexName == indexName);
        return Task.CompletedTask;
    }

    public Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task UpsertVectorsAsync(string indexName, List<EmbeddingVector> vectors, CancellationToken cancellationToken = default)
    {
        foreach (var v in vectors)
        {
            var existing = _store.Find(s => s.IndexName == indexName && s.Entry.ChunkId == v.ChunkId);
            _store.RemoveAll(s => s.IndexName == indexName && s.Entry.ChunkId == v.ChunkId);
            _store.Add((indexName, new IndexEntry
            {
                Id = v.Id, ChunkId = v.ChunkId, DocumentId = v.DocumentId, Vector = v.Vector
            }));
        }
        return Task.CompletedTask;
    }

    public Task DeleteVectorsAsync(string indexName, List<string> chunkIds, CancellationToken cancellationToken = default)
    {
        _store.RemoveAll(s => s.IndexName == indexName && chunkIds.Contains(s.Entry.ChunkId));
        return Task.CompletedTask;
    }

    public Task DeleteVectorsByDocumentAsync(string indexName, string documentId, CancellationToken cancellationToken = default)
    {
        _store.RemoveAll(s => s.IndexName == indexName && s.Entry.DocumentId == documentId);
        return Task.CompletedTask;
    }

    public Task<List<SearchResult>> SemanticSearchAsync(string indexName, float[] queryVector, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default)
    {
        var entries = _store.Where(s => s.IndexName == indexName).Select(s => s.Entry).ToList();
        if (metadataFilters != null)
            entries = entries.Where(e => e.Metadata != null && metadataFilters.All(f => e.Metadata.TryGetValue(f.Key, out var v) && v == f.Value)).ToList();

        var results = entries.Select(e => new SearchResult(
            e.DocumentId, e.ChunkId, e.Content ?? string.Empty,
            CosineSimilarity(queryVector, e.Vector),
            e.DocumentName, e.Format, e.PageNumber, e.Section, e.Metadata,
            new ModelsCitation(e.DocumentName, e.Section, e.PageNumber, e.ChunkId,
                CosineSimilarity(queryVector, e.Vector), null,
                (e.Content ?? string.Empty).Length > 200 ? (e.Content ?? string.Empty)[..200] : e.Content ?? string.Empty)
        )).OrderByDescending(r => r.Score).Take(topK).ToList();

        return Task.FromResult(results);
    }

    public Task<List<SearchResult>> HybridSearchAsync(string indexName, float[] queryVector, string keywordQuery, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, double semanticWeight = 0.7, CancellationToken cancellationToken = default)
    {
        return SemanticSearchAsync(indexName, queryVector, topK, scoreThreshold, metadataFilters, cancellationToken);
    }

    public Task<List<SearchResult>> KeywordSearchAsync(string indexName, string query, int topK = 10, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default)
    {
        var terms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var entries = _store.Where(s => s.IndexName == indexName).Select(s => s.Entry).ToList();

        var results = entries
            .Select(e => (Score: terms.Count(t => (e.Content ?? string.Empty).ToLower().Contains(t)) / (double)Math.Max(terms.Length, 1), e))
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => new SearchResult(x.e.DocumentId, x.e.ChunkId, x.e.Content ?? string.Empty, x.Score,
                x.e.DocumentName, x.e.Format, x.e.PageNumber, x.e.Section, x.e.Metadata,
                new ModelsCitation(x.e.DocumentName, x.e.Section, x.e.PageNumber, x.e.ChunkId, x.Score, null,
                    (x.e.Content ?? string.Empty)[..Math.Min(200, (x.e.Content ?? string.Empty).Length)])))
            .ToList();

        return Task.FromResult(results);
    }

    public Task<long> GetVectorCountAsync(string indexName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult((long)_store.Count(s => s.IndexName == indexName));
    }

    public Task<Dictionary<string, object>> GetIndexStatsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Dictionary<string, object> { ["vector_count"] = _store.Count(s => s.IndexName == indexName) });
    }

    private static double CosineSimilarity(float[] v1, float[] v2)
    {
        double dot = 0, m1 = 0, m2 = 0;
        for (int i = 0; i < Math.Min(v1.Length, v2.Length); i++)
        {
            dot += v1[i] * v2[i];
            m1 += v1[i] * v1[i];
            m2 += v2[i] * v2[i];
        }
        var mag = Math.Sqrt(m1) * Math.Sqrt(m2);
        return mag < double.Epsilon ? 0 : dot / mag;
    }
}
