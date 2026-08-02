using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.VectorStores;

public class MilvusVectorStore : BaseVectorStore
{
    public override VectorStoreType StoreType => VectorStoreType.Milvus;
    public override string StoreName => "Milvus";

    public override Task CreateIndexAsync(string indexName, int dimensions, string? distanceMetric = "cosine", CancellationToken cancellationToken = default)
    {
        InMemoryIndices[indexName] = new InMemoryIndex { Name = indexName, Dimensions = dimensions, DistanceMetric = distanceMetric ?? "cosine" };
        return Task.CompletedTask;
    }

    public override Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default) { InMemoryIndices.Remove(indexName); return Task.CompletedTask; }

    public override Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default) =>
        Task.FromResult(InMemoryIndices.ContainsKey(indexName));

    public override Task UpsertVectorsAsync(string indexName, List<EmbeddingVector> vectors, CancellationToken cancellationToken = default)
    {
        if (!InMemoryIndices.TryGetValue(indexName, out var index)) throw new InvalidOperationException($"Index '{indexName}' not found");
        foreach (var v in vectors)
        {
            var existing = index.Entries.Find(e => e.ChunkId == v.ChunkId);
            if (existing != null) existing.Vector = v.Vector;
            else index.Entries.Add(new IndexEntry { Id = v.Id, ChunkId = v.ChunkId, DocumentId = v.DocumentId, Vector = v.Vector });
        }
        return Task.CompletedTask;
    }

    public override Task DeleteVectorsAsync(string indexName, List<string> chunkIds, CancellationToken cancellationToken = default)
    {
        if (InMemoryIndices.TryGetValue(indexName, out var index)) index.Entries.RemoveAll(e => chunkIds.Contains(e.ChunkId));
        return Task.CompletedTask;
    }

    public override Task DeleteVectorsByDocumentAsync(string indexName, string documentId, CancellationToken cancellationToken = default)
    {
        if (InMemoryIndices.TryGetValue(indexName, out var index)) index.Entries.RemoveAll(e => e.DocumentId == documentId);
        return Task.CompletedTask;
    }

    public override Task<List<SearchResult>> SemanticSearchAsync(string indexName, float[] queryVector, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default)
    {
        if (!InMemoryIndices.TryGetValue(indexName, out var index)) return Task.FromResult(new List<SearchResult>());
        var filtered = metadataFilters != null ? index.Entries.Where(e => e.Metadata != null && metadataFilters.All(f => e.Metadata.TryGetValue(f.Key, out var v) && v == f.Value)).ToList() : index.Entries;
        var scored = filtered.Select(e => (Score: CosineSimilarity(queryVector, e.Vector), e)).ToList();
        var results = ApplyScoreThreshold(scored.Select(s => (s.Score, new SearchResult(s.e.DocumentId, s.e.ChunkId, s.e.Content, s.Score, s.e.DocumentName, s.e.Format, s.e.PageNumber, s.e.Section, s.e.Metadata, new Citation(s.e.DocumentName, s.e.Section, s.e.PageNumber, s.e.ChunkId, s.Score, null, s.e.Content[..Math.Min(200, s.e.Content.Length)])))).ToList(), scoreThreshold);
        return Task.FromResult(results.Take(topK).ToList());
    }

    public override Task<List<SearchResult>> HybridSearchAsync(string indexName, float[] queryVector, string keywordQuery, int topK = 10, double? scoreThreshold = null, Dictionary<string, string>? metadataFilters = null, double semanticWeight = 0.7, CancellationToken cancellationToken = default)
    {
        if (!InMemoryIndices.TryGetValue(indexName, out var index)) return Task.FromResult(new List<SearchResult>());
        var terms = keywordQuery.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var filtered = metadataFilters != null ? index.Entries.Where(e => e.Metadata != null && metadataFilters.All(f => e.Metadata.TryGetValue(f.Key, out var v) && v == f.Value)).ToList() : index.Entries;
        var scored = filtered.Select(e => { var ss = CosineSimilarity(queryVector, e.Vector); var ks = terms.Any() ? terms.Count(t => e.Content.ToLower().Contains(t)) / (double)terms.Length : 0; return (Score: ss * semanticWeight + ks * (1 - semanticWeight), e); }).ToList();
        var results = ApplyScoreThreshold(scored.Select(s => (s.Score, new SearchResult(s.e.DocumentId, s.e.ChunkId, s.e.Content, s.Score, s.e.DocumentName, s.e.Format, s.e.PageNumber, s.e.Section, s.e.Metadata, new Citation(s.e.DocumentName, s.e.Section, s.e.PageNumber, s.e.ChunkId, s.Score, null, s.e.Content[..Math.Min(200, s.e.Content.Length)])))).ToList(), scoreThreshold);
        return Task.FromResult(results.Take(topK).ToList());
    }

    public override Task<List<SearchResult>> KeywordSearchAsync(string indexName, string query, int topK = 10, Dictionary<string, string>? metadataFilters = null, CancellationToken cancellationToken = default)
    {
        if (!InMemoryIndices.TryGetValue(indexName, out var index)) return Task.FromResult(new List<SearchResult>());
        var terms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var filtered = metadataFilters != null ? index.Entries.Where(e => e.Metadata != null && metadataFilters.All(f => e.Metadata.TryGetValue(f.Key, out var v) && v == f.Value)).ToList() : index.Entries;
        var results = filtered.Select(e => (Score: terms.Any() ? terms.Count(t => e.Content.ToLower().Contains(t)) / (double)terms.Length : 0, e)).Where(s => s.Score > 0).OrderByDescending(s => s.Score).Take(topK).Select(s => new SearchResult(s.e.DocumentId, s.e.ChunkId, s.e.Content, s.Score, s.e.DocumentName, s.e.Format, s.e.PageNumber, s.e.Section, s.e.Metadata, new Citation(s.e.DocumentName, s.e.Section, s.e.PageNumber, s.e.ChunkId, s.Score, null, s.e.Content[..Math.Min(200, s.e.Content.Length)]))).ToList();
        return Task.FromResult(results);
    }

    public override Task<long> GetVectorCountAsync(string indexName, CancellationToken cancellationToken = default) =>
        Task.FromResult((long)(InMemoryIndices.TryGetValue(indexName, out var index) ? index.Entries.Count : 0));

    public override Task<Dictionary<string, object>> GetIndexStatsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        if (!InMemoryIndices.TryGetValue(indexName, out var index)) return Task.FromResult(new Dictionary<string, object>());
        return Task.FromResult(new Dictionary<string, object> { ["name"] = index.Name, ["dimensions"] = index.Dimensions, ["distance_metric"] = index.DistanceMetric, ["vector_count"] = index.Entries.Count, ["document_count"] = index.Entries.Select(e => e.DocumentId).Distinct().Count() });
    }
}
