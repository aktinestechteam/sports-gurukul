using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Embedding;

internal sealed class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingProviderFactory _factory;
    private readonly KnowledgePlatformOptions _options;
    private readonly EmbeddingCache _cache;

    public EmbeddingService(
        IEmbeddingProviderFactory factory,
        KnowledgePlatformOptions options)
    {
        _factory = factory;
        _options = options;
        _cache = new EmbeddingCache(options.Embedding);
    }

    public IEmbeddingProvider Provider => _factory.GetProvider();

    public async Task<IReadOnlyList<ChunkEmbedding>> EmbedChunksAsync(
        IReadOnlyList<DocumentChunk> chunks,
        string tenantId,
        string ownerUserId,
        CancellationToken ct = default)
    {
        var provider = _factory.GetProvider();
        var uniqueTexts = chunks.Select(c => c.Text).Distinct(StringComparer.Ordinal).ToList();
        var embeddings = new Dictionary<string, EmbeddingVector>(StringComparer.Ordinal);

        var uncached = uniqueTexts.Where(t => !embeddings.ContainsKey(t) && _cache.TryGet(t) is null).ToList();
        foreach (var text in uniqueTexts)
        {
            var cached = _cache.TryGet(text);
            if (cached is not null)
            {
                embeddings[text] = cached;
            }
        }

        if (uncached.Count > 0)
        {
            var batchSize = Math.Max(1, _options.Embedding.BatchSize);
            for (var i = 0; i < uncached.Count; i += batchSize)
            {
                ct.ThrowIfCancellationRequested();
                var batch = uncached.Skip(i).Take(batchSize).ToList();
                var batchVectors = await provider.EmbedBatchAsync(batch, ct);
                for (var j = 0; j < batch.Count; j++)
                {
                    embeddings[batch[j]] = batchVectors[j];
                    _cache.Set(batch[j], batchVectors[j]);
                }
            }
        }

        var result = new List<ChunkEmbedding>(chunks.Count);
        foreach (var chunk in chunks)
        {
            ct.ThrowIfCancellationRequested();
            if (!embeddings.TryGetValue(chunk.Text, out var vector))
            {
                vector = await provider.EmbedAsync(chunk.Text, ct);
                _cache.Set(chunk.Text, vector);
            }

            result.Add(new ChunkEmbedding(chunk.Id, chunk.IndexName, vector, chunk, tenantId, ownerUserId));
        }

        return result;
    }

    public async Task<EmbeddingVector> EmbedQueryAsync(string query, CancellationToken ct = default)
    {
        var cached = _cache.TryGet(query);
        if (cached is not null)
        {
            return cached;
        }

        var provider = _factory.GetProvider();
        var vector = await provider.EmbedAsync(query, ct);
        _cache.Set(query, vector);
        return vector;
    }
}
