using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.RecommendedEvents;

public class RecommendedEventsQueryHandler : IRequestHandler<RecommendedEventsQuery, Result<IReadOnlyList<RecommendationDto>>>
{
    private readonly IRecommendationEngine _recommendationEngine;
    private readonly IPersonalizationService _personalizationService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<RecommendedEventsQueryHandler> _logger;
    private const string CachePrefix = "recommended_events_";

    public RecommendedEventsQueryHandler(
        IRecommendationEngine recommendationEngine,
        IPersonalizationService personalizationService,
        ICacheService cacheService,
        ILogger<RecommendedEventsQueryHandler> logger)
    {
        _recommendationEngine = recommendationEngine;
        _personalizationService = personalizationService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RecommendationDto>>> Handle(RecommendedEventsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting recommended events for user {UserId}", request.UserId);

        if (request.UserId.HasValue)
        {
            var cacheKey = $"{CachePrefix}{request.UserId.Value}_{request.Limit}";
            var cached = await _cacheService.GetAsync<List<RecommendationDto>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return Result<IReadOnlyList<RecommendationDto>>.Success(cached);
            }

            var result = await _recommendationEngine.GetPersonalizedRecommendationsAsync(
                request.UserId.Value, request.Limit, cancellationToken);

            var list = result.ToList();
            await _cacheService.SetAsync(cacheKey, list, TimeSpan.FromMinutes(30), cancellationToken);
            return Result<IReadOnlyList<RecommendationDto>>.Success(list);
        }

        var preferences = await _personalizationService.GetUserPreferencesAsync(Guid.Empty, cancellationToken);
        var sports = request.PreferredSports?.ToList() ?? preferences.PreferredSports.ToList();
        var eventTypes = request.PreferredEventTypes?.ToList() ?? preferences.PreferredEventTypes.ToList();

        var genericResult = await _recommendationEngine.GetRecommendationsAsync(
            null, request.City ?? preferences.PreferredCity,
            sports, eventTypes,
            request.Latitude ?? preferences.PreferredLatitude,
            request.Longitude ?? preferences.PreferredLongitude,
            request.Limit, cancellationToken);

        return Result<IReadOnlyList<RecommendationDto>>.Success(genericResult);
    }
}
