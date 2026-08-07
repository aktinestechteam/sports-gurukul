using System.Collections.Concurrent;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.VectorStores;

internal sealed class InMemoryVectorStore : IVectorStore
{
    public string Name => "inmemory";

    public VectorStoreCapabilities Capabilities => new(true, true, true);

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, ChunkEmbedding>> _collections = new(StringComparer.Ordinal);

    public Task UpsertAsync(ChunkEmbedding embedding, CancellationToken ct = default)
    {
        var collection = _collections.GetOrAdd(embedding.IndexName, _ => new ConcurrentDictionary<Guid, ChunkEmbedding>());
        collection[embedding.ChunkId] = embedding;
        return Task.CompletedTask;
    }

    public Task UpsertBatchAsync(IReadOnlyList<ChunkEmbedding> embeddings, CancellationToken ct = default)
    {
        foreach (var embedding in embeddings)
        {
            ct.ThrowIfCancellationRequested();
            var collection = _collections.GetOrAdd(embedding.IndexName, _ => new ConcurrentDictionary<Guid, ChunkEmbedding>());
            collection[embedding.ChunkId] = embedding;
        }

        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(Guid chunkId, CancellationToken ct = default)
    {
        foreach (var collection in _collections.Values)
        {
            if (collection.TryRemove(chunkId, out _))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task DeleteBatchAsync(IReadOnlyList<Guid> chunkIds, CancellationToken ct = default)
    {
        var ids = chunkIds.ToHashSet();
        foreach (var collection in _collections.Values)
        {
            foreach (var id in ids)
            {
                collection.TryRemove(id, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task<int> DeleteByFilterAsync(VectorFilter filter, CancellationToken ct = default)
    {
        if (!_collections.TryGetValue(filter.IndexName, out var collection))
        {
            return Task.FromResult(0);
        }

        var removed = 0;
        foreach (var (id, embedding) in collection)
        {
            if (MatchesFilter(embedding, filter))
            {
                collection.TryRemove(id, out _);
                removed++;
            }
        }

        return Task.FromResult(removed);
    }

    public Task<IReadOnlyList<RetrievedChunk>> SearchAsync(VectorSearchQuery query, CancellationToken ct = default)
    {
        if (!_collections.TryGetValue(query.Filter.IndexName, out var collection))
        {
            return Task.FromResult<IReadOnlyList<RetrievedChunk>>(Array.Empty<RetrievedChunk>());
        }

        var excluded = query.ExcludeChunkIds?.ToHashSet() ?? new HashSet<Guid>();
        var results = new List<(ChunkEmbedding Embedding, float Score)>();
        foreach (var (id, embedding) in collection)
        {
            ct.ThrowIfCancellationRequested();
            if (excluded.Contains(id) || !MatchesFilter(embedding, query.Filter))
            {
                continue;
            }

            var score = VectorMath.CosineSimilarity(query.Vector.Values, embedding.Vector.Values);
            if (score >= query.MinScore)
            {
                results.Add((embedding, score));
            }
        }

        var ranked = results
            .OrderByDescending(r => r.Score)
            .Take(Math.Max(0, query.TopK))
            .Select((r, i) => new RetrievedChunk(r.Embedding.Chunk, r.Score, i, RetrievalStrategy.Semantic))
            .ToList();

        return Task.FromResult<IReadOnlyList<RetrievedChunk>>(ranked);
    }

    public Task<IReadOnlyList<RetrievedChunk>> SearchByTextAsync(KeywordSearchQuery query, CancellationToken ct = default)
    {
        if (!_collections.TryGetValue(query.Filter.IndexName, out var collection))
        {
            return Task.FromResult<IReadOnlyList<RetrievedChunk>>(Array.Empty<RetrievedChunk>());
        }

        var queryTokens = VectorMath.Tokenize(query.QueryText);
        if (queryTokens.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<RetrievedChunk>>(Array.Empty<RetrievedChunk>());
        }

        var excluded = query.ExcludeChunkIds?.ToHashSet() ?? new HashSet<Guid>();
        var candidates = collection.Values.Where(e => MatchesFilter(e, query.Filter)).ToList();
        var documentFrequency = BuildDocumentFrequency(candidates);
        var averageLength = candidates.Count == 0
            ? 1.0
            : candidates.Average(c => Math.Max(1, VectorMath.Tokenize(c.Chunk.Text).Count));
        var results = new List<(ChunkEmbedding Embedding, float Score)>();

        foreach (var (id, embedding) in collection)
        {
            ct.ThrowIfCancellationRequested();
            if (excluded.Contains(id) || !MatchesFilter(embedding, query.Filter))
            {
                continue;
            }

            var score = Bm25Score(embedding.Chunk.Text, queryTokens, documentFrequency, collection.Count, averageLength);
            if (score > 0f && score >= query.MinScore)
            {
                results.Add((embedding, score));
            }
        }

        var ranked = results
            .OrderByDescending(r => r.Score)
            .Take(Math.Max(0, query.TopK))
            .Select((r, i) => new RetrievedChunk(r.Embedding.Chunk, r.Score, i, RetrievalStrategy.Keyword))
            .ToList();

        return Task.FromResult<IReadOnlyList<RetrievedChunk>>(ranked);
    }

    public Task<long> CountAsync(string? indexName = null, CancellationToken ct = default)
    {
        if (indexName is null)
        {
            return Task.FromResult<long>(_collections.Values.Sum(c => c.Count));
        }

        return Task.FromResult<long>(_collections.TryGetValue(indexName, out var collection) ? collection.Count : 0);
    }

    public Task ResetAsync(string? indexName = null, CancellationToken ct = default)
    {
        if (indexName is null)
        {
            _collections.Clear();
        }
        else
        {
            _collections.TryRemove(indexName, out _);
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsHealthyAsync(CancellationToken ct = default) => Task.FromResult(true);

    private static bool MatchesFilter(ChunkEmbedding embedding, VectorFilter filter)
    {
        if (!string.Equals(filter.IndexName, embedding.IndexName, StringComparison.Ordinal))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(filter.TenantId)
            && !string.Equals(filter.TenantId, embedding.TenantId, StringComparison.Ordinal))
        {
            return false;
        }

        if (filter.DocumentIds is { Count: > 0 }
            && !filter.DocumentIds.Contains(embedding.Chunk.DocumentId))
        {
            return false;
        }

        if (filter.Categories is { Count: > 0 })
        {
            var categories = filter.Categories.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var classification = GetMetadata(embedding, "classification");
            var documentType = GetMetadata(embedding, "documentType");
            if (!categories.Contains(classification) && !categories.Contains(documentType))
            {
                return false;
            }
        }

        if (filter.Metadata is { Count: > 0 })
        {
            foreach (var (key, value) in filter.Metadata)
            {
                if (!string.Equals(GetMetadata(embedding, key), value, StringComparison.Ordinal))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static string GetMetadata(ChunkEmbedding embedding, string key)
    {
        if (embedding.Chunk.Metadata is { } metadata
            && metadata.TryGetValue(key, out var value))
        {
            return value ?? string.Empty;
        }

        return string.Empty;
    }

    private static IReadOnlyDictionary<string, int> BuildDocumentFrequency(IEnumerable<ChunkEmbedding> documents)
    {
        var frequency = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var document in documents)
        {
            var tokens = VectorMath.Tokenize(document.Chunk.Text).Distinct();
            foreach (var token in tokens)
            {
                frequency[token] = frequency.TryGetValue(token, out var count) ? count + 1 : 1;
            }
        }

        return frequency;
    }

    private static float Bm25Score(
        string text,
        IReadOnlyList<string> queryTokens,
        IReadOnlyDictionary<string, int> documentFrequency,
        int totalDocuments,
        double averageLength)
    {
        const float k1 = 1.2f;
        const float b = 0.75f;

        var tokens = VectorMath.Tokenize(text);
        if (tokens.Count == 0)
        {
            return 0f;
        }

        var termFrequency = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var token in tokens)
        {
            termFrequency[token] = termFrequency.TryGetValue(token, out var count) ? count + 1 : 1;
        }

        var docLength = tokens.Count;
        double score = 0;

        foreach (var token in queryTokens.Distinct())
        {
            if (!termFrequency.TryGetValue(token, out var tf) || tf == 0)
            {
                continue;
            }

            var df = documentFrequency.TryGetValue(token, out var freq) ? freq : 1;
            var idf = Math.Log(1.0 + ((totalDocuments - df + 0.5) / (df + 0.5)));
            var denominator = tf + k1 * (1 - b + b * (docLength / averageLength));
            score += idf * ((tf * (k1 + 1)) / denominator);
        }

        return (float)score;
    }
}
