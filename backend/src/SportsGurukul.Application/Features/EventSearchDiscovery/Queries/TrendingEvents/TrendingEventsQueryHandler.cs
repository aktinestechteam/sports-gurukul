using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.TrendingEvents;

public class TrendingEventsQueryHandler : IRequestHandler<TrendingEventsQuery, Result<IReadOnlyList<TrendingEventDto>>>
{
    private readonly IRecommendationEngine _recommendationEngine;
    private readonly ICacheService _cacheService;
    private readonly ILogger<TrendingEventsQueryHandler> _logger;
    private const string CachePrefix = "trending_events_";

    public TrendingEventsQueryHandler(
        IRecommendationEngine recommendationEngine,
        ICacheService cacheService,
        ILogger<TrendingEventsQueryHandler> logger)
    {
        _recommendationEngine = recommendationEngine;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<TrendingEventDto>>> Handle(TrendingEventsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CachePrefix}{request.City ?? "all"}_{request.Limit}";
        var cached = await _cacheService.GetAsync<List<TrendingEventDto>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogInformation("Returning cached trending events");
            return Result<IReadOnlyList<TrendingEventDto>>.Success(cached);
        }

        _logger.LogInformation("Getting trending events: City={City}, Limit={Limit}", request.City, request.Limit);

        var result = await _recommendationEngine.GetTrendingEventsAsync(request.City, request.Limit, cancellationToken);

        var list = result.ToList();
        await _cacheService.SetAsync(cacheKey, list, TimeSpan.FromMinutes(15), cancellationToken);

        return Result<IReadOnlyList<TrendingEventDto>>.Success(list);
    }
}
