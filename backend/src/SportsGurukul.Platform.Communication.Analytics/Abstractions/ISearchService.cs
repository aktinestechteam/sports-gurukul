using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface ISearchService
{
    Task<UnifiedSearchResult> SearchAsync(UnifiedSearchRequest request, CancellationToken ct = default);
    Task<List<SearchSuggestionDto>> GetSuggestionsAsync(string query, SearchEntityType entityType, int maxResults = 10, CancellationToken ct = default);
    Task<List<SearchFacetDto>> GetFacetsAsync(string query, SearchEntityType entityType, CancellationToken ct = default);
    Task<UnifiedSearchResult> SearchTemplatesAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default);
    Task<UnifiedSearchResult> SearchCampaignsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default);
    Task<UnifiedSearchResult> SearchNotificationsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default);
    Task<UnifiedSearchResult> SearchAnalyticsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default);
    Task<UnifiedSearchResult> SearchSegmentsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default);
    Task IndexEntityAsync<T>(T entity, CancellationToken ct = default);
    Task RebuildIndexAsync(SearchEntityType entityType, CancellationToken ct = default);
    Task ClearIndexAsync(SearchEntityType entityType, CancellationToken ct = default);
}
