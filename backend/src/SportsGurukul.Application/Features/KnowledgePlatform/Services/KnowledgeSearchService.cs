using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class KnowledgeSearchService : IKnowledgeSearchService
{
    private readonly IVectorStoreFactory _storeFactory;
    private readonly IEmbeddingProviderFactory _embeddingFactory;
    private readonly IRerankerService _rerankerService;
    private readonly ICitationService _citationService;
    private readonly ILogger<KnowledgeSearchService> _logger;

    public KnowledgeSearchService(
        IVectorStoreFactory storeFactory,
        IEmbeddingProviderFactory embeddingFactory,
        IRerankerService rerankerService,
        ICitationService citationService,
        ILogger<KnowledgeSearchService> logger)
    {
        _storeFactory = storeFactory;
        _embeddingFactory = embeddingFactory;
        _rerankerService = rerankerService;
        _citationService = citationService;
        _logger = logger;
    }

    public async Task<RetrievalContext> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var embeddingProvider = _embeddingFactory.GetProvider(EmbeddingProviderType.OpenAI);
        var queryVector = await embeddingProvider.GenerateEmbeddingAsync(query.Text, "query", "", cancellationToken);

        var store = _storeFactory.GetStore(query.VectorStoreName ?? VectorStoreType.Qdrant.ToString());
        var indexName = "default";

        List<SearchResult> results;

        switch (query.RetrievalType)
        {
            case RetrievalType.Hybrid:
                results = await store.HybridSearchAsync(indexName, queryVector.Vector, query.Text,
                    query.TopK, query.ScoreThreshold, query.MetadataFilters, cancellationToken: cancellationToken);
                break;
            case RetrievalType.Keyword:
                results = await store.KeywordSearchAsync(indexName, query.Text,
                    query.TopK, query.MetadataFilters, cancellationToken);
                break;
            default:
                results = await store.SemanticSearchAsync(indexName, queryVector.Vector,
                    query.TopK, query.ScoreThreshold, query.MetadataFilters, cancellationToken);
                break;
        }

        sw.Stop();

        return new RetrievalContext(
            Results: results,
            OriginalQuery: query.Text,
            ExpandedQuery: null,
            DurationMs: sw.ElapsedMilliseconds,
            TotalResults: results.Count
        );
    }

    public async Task<RetrievalContext> MultiKnowledgeSearchAsync(
        List<SearchQuery> queries,
        RetrievalType mergeStrategy = RetrievalType.Hybrid,
        CancellationToken cancellationToken = default)
    {
        var allResults = new List<SearchResult>();

        foreach (var query in queries)
        {
            var context = await SearchAsync(query, cancellationToken);
            allResults.AddRange(context.Results);
        }

        var merged = mergeStrategy switch
        {
            RetrievalType.Semantic => allResults.OrderByDescending(r => r.Score).ToList(),
            RetrievalType.Hybrid => allResults.OrderByDescending(r => r.Score).ToList(),
            _ => allResults.GroupBy(r => r.ChunkId).Select(g => g.First()).OrderByDescending(r => r.Score).ToList()
        };

        return new RetrievalContext(
            Results: merged,
            OriginalQuery: string.Join(" | ", queries.Select(q => q.Text)),
            ExpandedQuery: null,
            DurationMs: 0,
            TotalResults: merged.Count
        );
    }

    public async Task<List<SearchResult>> SemanticSearchAsync(
        string indexName, string text, int topK = 10,
        double? scoreThreshold = null,
        Dictionary<string, string>? metadataFilters = null,
        CancellationToken cancellationToken = default)
    {
        var ctx = await SearchAsync(new SearchQuery(
            Text: text,
            RetrievalType: RetrievalType.Semantic,
            TopK: topK,
            ScoreThreshold: scoreThreshold,
            MetadataFilters: metadataFilters,
            VectorStoreName: null,
            EmbeddingModel: null
        ), cancellationToken);

        return ctx.Results;
    }

    public async Task<List<SearchResult>> HybridSearchAsync(
        string indexName, string text, int topK = 10,
        double? scoreThreshold = null,
        Dictionary<string, string>? metadataFilters = null,
        CancellationToken cancellationToken = default)
    {
        var ctx = await SearchAsync(new SearchQuery(
            Text: text,
            RetrievalType: RetrievalType.Hybrid,
            TopK: topK,
            ScoreThreshold: scoreThreshold,
            MetadataFilters: metadataFilters,
            VectorStoreName: null,
            EmbeddingModel: null
        ), cancellationToken);

        return ctx.Results;
    }

    public async Task<List<SearchResult>> KeywordSearchAsync(
        string indexName, string query, int topK = 10,
        Dictionary<string, string>? metadataFilters = null,
        CancellationToken cancellationToken = default)
    {
        var ctx = await SearchAsync(new SearchQuery(
            Text: query,
            RetrievalType: RetrievalType.Keyword,
            TopK: topK,
            MetadataFilters: metadataFilters,
            VectorStoreName: null,
            EmbeddingModel: null
        ), cancellationToken);

        return ctx.Results;
    }

    public async Task<List<SearchResult>> SearchWithRerankingAsync(
        string indexName, string text, int topK = 10,
        int rerankTopK = 5,
        CancellationToken cancellationToken = default)
    {
        var results = await SemanticSearchAsync(indexName, text, topK * 2, null, null, cancellationToken);
        var reranked = await _rerankerService.RerankResultsAsync(text, results, rerankTopK, cancellationToken);

        return reranked.Select(r => new SearchResult(
            DocumentId: r.DocumentId,
            ChunkId: r.ChunkId,
            Content: r.Content,
            Score: r.RerankedScore,
            DocumentName: "",
            Format: DocumentFormat.PlainText,
            PageNumber: null,
            Section: null,
            Metadata: null,
            Citation: new Citation("", null, null, r.ChunkId, r.RerankedScore, null, r.Content[..Math.Min(200, r.Content.Length)])
        )).ToList();
    }
}
