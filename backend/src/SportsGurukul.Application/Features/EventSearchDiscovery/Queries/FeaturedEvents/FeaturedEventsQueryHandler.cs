using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.FeaturedEvents;

public class FeaturedEventsQueryHandler : IRequestHandler<FeaturedEventsQuery, Result<IReadOnlyList<FeaturedEventDto>>>
{
    private readonly IRecommendationEngine _recommendationEngine;
    private readonly ICacheService _cacheService;
    private readonly ILogger<FeaturedEventsQueryHandler> _logger;
    private const string CachePrefix = "featured_events_";

    public FeaturedEventsQueryHandler(
        IRecommendationEngine recommendationEngine,
        ICacheService cacheService,
        ILogger<FeaturedEventsQueryHandler> logger)
    {
        _recommendationEngine = recommendationEngine;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<FeaturedEventDto>>> Handle(FeaturedEventsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CachePrefix}{request.City ?? "all"}_{request.SportName ?? "all"}_{request.Limit}";
        var cached = await _cacheService.GetAsync<List<FeaturedEventDto>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogInformation("Returning cached featured events");
            return Result<IReadOnlyList<FeaturedEventDto>>.Success(cached);
        }

        _logger.LogInformation("Getting featured events: City={City}, Sport={Sport}", request.City, request.SportName);

        var result = await _recommendationEngine.GetFeaturedEventsAsync(request.City, request.SportName, request.Limit, cancellationToken);

        var list = result.ToList();
        await _cacheService.SetAsync(cacheKey, list, TimeSpan.FromMinutes(15), cancellationToken);

        return Result<IReadOnlyList<FeaturedEventDto>>.Success(list);
    }
}
