using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class RerankerService : IRerankerService
{
    private readonly IReranker? _reranker;
    private readonly ILogger<RerankerService> _logger;

    public RerankerService(
        ILogger<RerankerService> logger,
        IReranker? reranker = null)
    {
        _reranker = reranker;
        _logger = logger;
    }

    public async Task<List<RerankingResult>> RerankResultsAsync(
        string query,
        List<SearchResult> results,
        int topK = 10,
        CancellationToken cancellationToken = default)
    {
        if (_reranker == null)
            return DefaultRerank(query, results, topK);

        return await _reranker.RerankAsync(query, results, topK, cancellationToken);
    }

    private List<RerankingResult> DefaultRerank(string query, List<SearchResult> results, int topK)
    {
        var terms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var reranked = results.Select(r =>
        {
            var keywordBoost = terms.Any()
                ? terms.Count(t => r.Content.ToLower().Contains(t)) * 0.05
                : 0;

            var positionPenalty = results.IndexOf(r) * 0.01;
            var rerankedScore = r.Score + keywordBoost - positionPenalty;

            return new RerankingResult(
                DocumentId: r.DocumentId,
                ChunkId: r.ChunkId,
                Content: r.Content,
                OriginalScore: r.Score,
                RerankedScore: Math.Clamp(rerankedScore, 0, 1)
            );
        })
        .OrderByDescending(r => r.RerankedScore)
        .Take(topK)
        .ToList();

        return reranked;
    }
}
