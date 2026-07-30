namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public enum SearchEntityType
{
    Template,
    Campaign,
    Notification,
    Analytics,
    Segment,
    Schedule,
    Provider,
    All
}

public record UnifiedSearchRequest(
    string Query,
    SearchEntityType EntityType,
    Dictionary<string, string>? Filters,
    string? SortBy,
    bool SortDescending,
    string? Locale,
    int PageNumber = 1,
    int PageSize = 20
);

public record UnifiedSearchResult(
    string Query,
    SearchEntityType EntityType,
    int TotalResults,
    int PageNumber,
    int PageSize,
    bool HasNextPage,
    long SearchTimeMs,
    List<SearchResultItemDto> Results
);

public record SearchResultItemDto(
    Guid Id,
    SearchEntityType EntityType,
    string Title,
    string? Description,
    string? Preview,
    string? Status,
    string? Channel,
    double? RelevanceScore,
    List<string>? MatchedFields,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    string? Url
);

public record SearchSuggestionDto(
    string Text,
    SearchEntityType EntityType,
    int ResultCount,
    double RelevanceScore
);

public record SearchFacetDto(
    string Field,
    List<SearchFacetValueDto> Values
);

public record SearchFacetValueDto(
    string Value,
    int Count
);
