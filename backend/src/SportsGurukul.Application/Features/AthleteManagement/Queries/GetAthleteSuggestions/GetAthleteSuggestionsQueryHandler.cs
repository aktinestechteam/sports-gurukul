using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSuggestions;

public class GetAthleteSuggestionsQueryHandler : IRequestHandler<GetAthleteSuggestionsQuery, Result<IReadOnlyList<AthleteSearchSuggestionDto>>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetAthleteSuggestionsQueryHandler> _logger;

    private const string CachePrefix = "athlete_suggestions_";

    public GetAthleteSuggestionsQueryHandler(
        IAthleteRepository athleteRepository,
        ICacheService cacheService,
        ILogger<GetAthleteSuggestionsQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AthleteSearchSuggestionDto>>> Handle(
        GetAthleteSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CachePrefix}{request.Prefix.ToLowerInvariant()}_{request.Limit}";

        var cached = await _cacheService.GetAsync<List<AthleteSearchSuggestionDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Result<IReadOnlyList<AthleteSearchSuggestionDto>>.Success(cached);

        _logger.LogInformation("Fetching suggestions for prefix: {Prefix}", request.Prefix);

        var suggestions = await _athleteRepository.GetSearchSuggestionsAsync(
            request.Prefix, request.Limit, cancellationToken);

        await _cacheService.SetAsync(cacheKey, suggestions.ToList(), TimeSpan.FromMinutes(5), cancellationToken);

        return Result<IReadOnlyList<AthleteSearchSuggestionDto>>.Success(suggestions);
    }
}
