using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class RetrievalService : IRetrievalService
{
    private readonly IKnowledgeSearchService _searchService;
    private readonly IRerankerService _rerankerService;
    private readonly ICitationService _citationService;
    private readonly ILogger<RetrievalService> _logger;

    public RetrievalService(
        IKnowledgeSearchService searchService,
        IRerankerService rerankerService,
        ICitationService citationService,
        ILogger<RetrievalService> logger)
    {
        _searchService = searchService;
        _rerankerService = rerankerService;
        _citationService = citationService;
        _logger = logger;
    }

    public async Task<RetrievalContext> RetrieveAsync(
        string indexName,
        string query,
        RetrievalType retrievalType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving for query: '{Query}' (type: {Type})", query, retrievalType);

        var context = await _searchService.SearchAsync(
            new SearchQuery(query, retrievalType),
            cancellationToken);

        if (context.Results.Count > 0)
        {
            var citations = _citationService.CreateCitations(context.Results);
        }

        return context;
    }

    public async Task<List<SearchResult>> RetrieveWithMetadataFilterAsync(
        string indexName,
        string query,
        Dictionary<string, string> filters,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving with metadata filters for: '{Query}'", query);

        var context = await _searchService.SearchAsync(
            new SearchQuery(query, RetrievalType.Semantic, MetadataFilters: filters),
            cancellationToken);

        return context.Results;
    }
}
