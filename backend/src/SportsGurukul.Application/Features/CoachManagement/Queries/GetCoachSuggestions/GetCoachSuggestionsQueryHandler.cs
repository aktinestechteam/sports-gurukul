using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSuggestions;

public class GetCoachSuggestionsQueryHandler : IRequestHandler<GetCoachSuggestionsQuery, Result<IReadOnlyList<CoachSearchSuggestionDto>>>
{
    private readonly ICoachSearchRepository _searchRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetCoachSuggestionsQueryHandler> _logger;

    private const string CachePrefix = "coach_suggestions_";

    public GetCoachSuggestionsQueryHandler(
        ICoachSearchRepository searchRepository,
        ICacheService cacheService,
        ILogger<GetCoachSuggestionsQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<CoachSearchSuggestionDto>>> Handle(
        GetCoachSuggestionsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prefix))
            return Result<IReadOnlyList<CoachSearchSuggestionDto>>.Success(Array.Empty<CoachSearchSuggestionDto>());

        var cacheKey = $"{CachePrefix}{request.Prefix.ToLowerInvariant()}_{request.Limit}";

        var cached = await _cacheService.GetAsync<List<CoachSearchSuggestionDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Result<IReadOnlyList<CoachSearchSuggestionDto>>.Success(cached);

        _logger.LogInformation("Fetching coach suggestions for prefix: {Prefix}", request.Prefix);

        var suggestions = await _searchRepository.GetSearchSuggestionsAsync(
            request.Prefix, request.Limit, cancellationToken);

        var dtos = suggestions.Select(s => new CoachSearchSuggestionDto
        {
            Text = s,
            Type = s.StartsWith("COACH-") ? "CoachCode" : "Name",
            SubText = s.StartsWith("COACH-") ? "Coach Code" : "Coach Name"
        }).ToList();

        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5), cancellationToken);

        return Result<IReadOnlyList<CoachSearchSuggestionDto>>.Success(dtos);
    }
}
