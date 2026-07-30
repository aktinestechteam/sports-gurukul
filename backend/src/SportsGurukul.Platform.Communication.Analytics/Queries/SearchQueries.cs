using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Queries;

public record UnifiedSearchQuery(UnifiedSearchRequest Request) : IRequest<UnifiedSearchResult>;

public class UnifiedSearchQueryHandler(ISearchService service) : IRequestHandler<UnifiedSearchQuery, UnifiedSearchResult>
{
    public Task<UnifiedSearchResult> Handle(UnifiedSearchQuery query, CancellationToken ct)
        => service.SearchAsync(query.Request, ct);
}

public record GetSearchSuggestionsQuery(string Query, SearchEntityType EntityType, int MaxResults = 10) : IRequest<List<SearchSuggestionDto>>;

public class GetSearchSuggestionsQueryHandler(ISearchService service) : IRequestHandler<GetSearchSuggestionsQuery, List<SearchSuggestionDto>>
{
    public Task<List<SearchSuggestionDto>> Handle(GetSearchSuggestionsQuery query, CancellationToken ct)
        => service.GetSuggestionsAsync(query.Query, query.EntityType, query.MaxResults, ct);
}

public record GetSearchFacetsQuery(string Query, SearchEntityType EntityType) : IRequest<List<SearchFacetDto>>;

public class GetSearchFacetsQueryHandler(ISearchService service) : IRequestHandler<GetSearchFacetsQuery, List<SearchFacetDto>>
{
    public Task<List<SearchFacetDto>> Handle(GetSearchFacetsQuery query, CancellationToken ct)
        => service.GetFacetsAsync(query.Query, query.EntityType, ct);
}
