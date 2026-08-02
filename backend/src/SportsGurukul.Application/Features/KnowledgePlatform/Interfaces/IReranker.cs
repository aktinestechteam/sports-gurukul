using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;

public interface IReranker
{
    Task<List<RerankingResult>> RerankAsync(string query, List<SearchResult> results, int topK = 10, CancellationToken cancellationToken = default);
    Task<List<RerankingResult>> RerankWithScoresAsync(string query, List<SearchResult> results, int topK = 10, CancellationToken cancellationToken = default);
}

public interface ICitationEngine
{
    Citation GenerateCitation(SearchResult result);
    List<Citation> GenerateCitations(List<SearchResult> results);
    string FormatCitationsAsMarkdown(List<Citation> citations);
    string FormatCitationsAsJson(List<Citation> citations);
}
