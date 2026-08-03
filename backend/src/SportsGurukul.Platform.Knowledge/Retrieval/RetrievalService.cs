using System.Diagnostics;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Retrieval;

internal sealed class RetrievalService : IRetrievalService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreFactory _vectorStoreFactory;
    private readonly KnowledgePlatformOptions _options;
    private readonly IKnowledgeMetricsCollector _metrics;
    private readonly IReadOnlyList<IReranker> _rerankers;

    public RetrievalService(
        IEmbeddingService embeddingService,
        IVectorStoreFactory vectorStoreFactory,
        KnowledgePlatformOptions options,
        IKnowledgeMetricsCollector metrics,
        IEnumerable<IReranker> rerankers)
    {
        _embeddingService = embeddingService;
        _vectorStoreFactory = vectorStoreFactory;
        _options = options;
        _metrics = metrics;
        _rerankers = rerankers.ToList();
    }

    public async Task<SearchResult> SearchAsync(KnowledgeSearchRequest request, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var mode = request.Mode == SearchMode.Hybrid
            ? SearchMode.Hybrid
            : request.Mode;

        IReadOnlyList<RetrievedChunk> vectorResults = Array.Empty<RetrievedChunk>();
        IReadOnlyList<RetrievedChunk> keywordResults = Array.Empty<RetrievedChunk>();

        if (mode is SearchMode.Vector or SearchMode.Hybrid)
        {
            vectorResults = await SearchVectorAsync(request, ct);
        }

        if (mode is SearchMode.Keyword or SearchMode.Hybrid)
        {
            keywordResults = await SearchKeywordAsync(request, ct);
        }

        var topK = request.TopK > 0 ? request.TopK : _options.Retrieval.DefaultTopK;
        IReadOnlyList<RetrievedChunk> final;

        if (mode == SearchMode.Hybrid)
        {
            var combined = vectorResults.Concat(keywordResults).ToList();
            final = UseRrf()
                ? await ApplyRerankerAsync("rrf", request.Query, combined, topK, ct)
                : FuseWeighted(vectorResults, keywordResults, topK);
        }
        else
        {
            var candidates = mode == SearchMode.Vector ? vectorResults : keywordResults;
            if (_options.Retrieval.EnableReRanking)
            {
                final = await ApplyRerankerAsync(PreferredReranker(), request.Query, candidates, topK, ct);
            }
            else
            {
                final = candidates.OrderByDescending(c => c.Score).Take(topK)
                    .Select((c, i) => c with { Rank = i }).ToList();
            }
        }

        stopwatch.Stop();
        _metrics.RecordSearch(request.IndexName, mode, final.Count, stopwatch.ElapsedMilliseconds, vectorResults.Count + keywordResults.Count);

        return new SearchResult(
            final,
            mode,
            vectorResults.Count + keywordResults.Count,
            stopwatch.ElapsedMilliseconds);
    }

    public async IAsyncEnumerable<RetrievedChunk> StreamAsync(KnowledgeSearchRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var result = await SearchAsync(request, ct);
        foreach (var chunk in result.Chunks)
        {
            ct.ThrowIfCancellationRequested();
            yield return chunk;
        }
    }

    private async Task<IReadOnlyList<RetrievedChunk>> SearchVectorAsync(KnowledgeSearchRequest request, CancellationToken ct)
    {
        var store = _vectorStoreFactory.GetStore();
        var queryVector = await _embeddingService.EmbedQueryAsync(request.Query, ct);
        var query = new VectorSearchQuery(
            queryVector,
            request.TopK > 0 ? request.TopK : _options.Retrieval.DefaultTopK,
            BuildFilter(request),
            request.MinScore);

        return await store.SearchAsync(query, ct);
    }

    private async Task<IReadOnlyList<RetrievedChunk>> SearchKeywordAsync(KnowledgeSearchRequest request, CancellationToken ct)
    {
        var store = _vectorStoreFactory.GetStore();
        if (!store.Capabilities.SupportsKeyword)
        {
            return Array.Empty<RetrievedChunk>();
        }

        var query = new KeywordSearchQuery(
            request.Query,
            request.TopK > 0 ? request.TopK : _options.Retrieval.DefaultTopK,
            BuildFilter(request),
            request.MinScore);

        return await store.SearchByTextAsync(query, ct);
    }

    private static VectorFilter BuildFilter(KnowledgeSearchRequest request) =>
        new(
            request.IndexName,
            request.TenantId,
            null,
            request.Categories,
            request.MetadataFilter);

    private IReadOnlyList<RetrievedChunk> FuseWeighted(
        IReadOnlyList<RetrievedChunk> vectorResults,
        IReadOnlyList<RetrievedChunk> keywordResults,
        int topK)
    {
        var vectorMax = vectorResults.Count > 0 ? vectorResults.Max(c => c.Score) : 0f;
        var keywordMax = keywordResults.Count > 0 ? keywordResults.Max(c => c.Score) : 0f;
        var vectorWeight = _options.Retrieval.VectorWeight;
        var keywordWeight = _options.Retrieval.KeywordWeight;

        var byId = new Dictionary<Guid, RetrievedChunk>();
        foreach (var result in vectorResults)
        {
            var normalized = vectorMax > 0 ? result.Score / vectorMax : 0f;
            byId[result.Chunk.Id] = result with { Score = normalized * vectorWeight };
        }

        foreach (var result in keywordResults)
        {
            var normalized = keywordMax > 0 ? result.Score / keywordMax : 0f;
            var contribution = normalized * keywordWeight;
            if (byId.TryGetValue(result.Chunk.Id, out var existing))
            {
                byId[result.Chunk.Id] = existing with { Score = existing.Score + contribution, SourceStrategy = RetrievalStrategy.Hybrid };
            }
            else
            {
                byId[result.Chunk.Id] = result with { Score = contribution, SourceStrategy = RetrievalStrategy.Hybrid };
            }
        }

        return byId.Values
            .OrderByDescending(c => c.Score)
            .Take(Math.Max(0, topK))
            .Select((c, i) => c with { Rank = i })
            .ToList();
    }

    private async Task<IReadOnlyList<RetrievedChunk>> ApplyRerankerAsync(
        string name,
        string query,
        IReadOnlyList<RetrievedChunk> candidates,
        int topK,
        CancellationToken ct)
    {
        var reranker = _rerankers.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase))
                       ?? _rerankers.FirstOrDefault(r => r.Name == "score");

        if (reranker is null)
        {
            return candidates.OrderByDescending(c => c.Score).Take(Math.Max(0, topK)).ToList();
        }

        var ranked = await reranker.RerankAsync(query, candidates, ct);
        return ranked.Take(Math.Max(0, topK)).ToList();
    }

    private string PreferredReranker() =>
        string.IsNullOrWhiteSpace(_options.Retrieval.Reranker) ? "score" : _options.Retrieval.Reranker;

    private bool UseRrf() =>
        _options.Retrieval.EnableReRanking
        && string.Equals(_options.Retrieval.Reranker, "rrf", StringComparison.OrdinalIgnoreCase);
}
