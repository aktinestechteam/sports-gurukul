using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceAvailability;

public class GetResourceAvailabilityQueryHandler
    : IRequestHandler<GetResourceAvailabilityQuery, Result<AvailabilityWindow>>
{
    private readonly IAvailabilityEngine _availabilityEngine;
    private readonly ILogger<GetResourceAvailabilityQueryHandler> _logger;

    public GetResourceAvailabilityQueryHandler(
        IAvailabilityEngine availabilityEngine,
        ILogger<GetResourceAvailabilityQueryHandler> logger)
    {
        _availabilityEngine = availabilityEngine;
        _logger = logger;
    }

    public async Task<Result<AvailabilityWindow>> Handle(
        GetResourceAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var context = new SchedulingContext
        {
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            TimeZoneId = request.TimeZoneId ?? "UTC"
        };

        var window = await _availabilityEngine.GetAvailabilityWindowAsync(
            request.ResourceId, request.ResourceType, request.Date.Date, context, cancellationToken);

        _logger.LogInformation(
            "Retrieved availability window for {ResourceType} {ResourceId} on {Date}: {Count} available, {Blocked} blocked",
            request.ResourceType, request.ResourceId, request.Date.Date,
            window.AvailableSlots.Count, window.BlockedSlots.Count);

        return Result<AvailabilityWindow>.Success(window);
    }
}
