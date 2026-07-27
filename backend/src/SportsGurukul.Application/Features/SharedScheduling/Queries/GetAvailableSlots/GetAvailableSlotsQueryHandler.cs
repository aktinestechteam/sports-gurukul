using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Queries.GetAvailableSlots;

public class GetAvailableSlotsQueryHandler
    : IRequestHandler<GetAvailableSlotsQuery, Result<IReadOnlyList<TimeSlot>>>
{
    private readonly IAvailabilityEngine _availabilityEngine;
    private readonly ILogger<GetAvailableSlotsQueryHandler> _logger;

    public GetAvailableSlotsQueryHandler(
        IAvailabilityEngine availabilityEngine,
        ILogger<GetAvailableSlotsQueryHandler> logger)
    {
        _availabilityEngine = availabilityEngine;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<TimeSlot>>> Handle(
        GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var context = new SchedulingContext
        {
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            TimeZoneId = request.TimeZoneId ?? "UTC"
        };

        var slots = await _availabilityEngine.GetAvailableSlotsAsync(
            request.ResourceId, request.ResourceType, request.Date.Date,
            context, request.SlotDuration, cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} available slots for {ResourceType} {ResourceId} on {Date}",
            slots.Count, request.ResourceType, request.ResourceId, request.Date.Date);

        return Result<IReadOnlyList<TimeSlot>>.Success(slots);
    }
}
