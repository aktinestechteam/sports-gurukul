using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Commands.TrackRecentlyViewed;

public class TrackRecentlyViewedCommandHandler : IRequestHandler<TrackRecentlyViewedCommand, Result<bool>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<TrackRecentlyViewedCommandHandler> _logger;

    public TrackRecentlyViewedCommandHandler(
        IEventSearchRepository searchRepository,
        ILogger<TrackRecentlyViewedCommandHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(TrackRecentlyViewedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tracking view for event {EventId} by user {UserId}", request.EventId, request.UserId);

        await _searchRepository.TrackViewAsync(
            request.EventId, request.UserId, request.Source, request.DeviceType, cancellationToken);

        return Result<bool>.Success(true);
    }
}
