using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.GenerateAvailableSlots;

public class GenerateAvailableSlotsCommandHandler
    : IRequestHandler<GenerateAvailableSlotsCommand, Result<IReadOnlyList<TimeSlot>>>
{
    private readonly IAvailabilityEngine _availabilityEngine;
    private readonly ILogger<GenerateAvailableSlotsCommandHandler> _logger;

    public GenerateAvailableSlotsCommandHandler(
        IAvailabilityEngine availabilityEngine,
        ILogger<GenerateAvailableSlotsCommandHandler> logger)
    {
        _availabilityEngine = availabilityEngine;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<TimeSlot>>> Handle(
        GenerateAvailableSlotsCommand request, CancellationToken cancellationToken)
    {
        var context = new SchedulingContext
        {
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            TimeZoneId = request.TimeZoneId ?? "UTC"
        };

        var allSlots = new List<TimeSlot>();
        var current = request.StartDate.Date;
        var end = request.EndDate.Date;

        while (current <= end)
        {
            var slots = await _availabilityEngine.GetAvailableSlotsAsync(
                request.ResourceId, request.ResourceType, current, context,
                request.SlotDuration, cancellationToken);
            allSlots.AddRange(slots);
            current = current.AddDays(1);
        }

        _logger.LogInformation(
            "Generated {Count} available slots for {ResourceType} {ResourceId} from {Start} to {End}",
            allSlots.Count, request.ResourceType, request.ResourceId, request.StartDate, request.EndDate);

        return Result<IReadOnlyList<TimeSlot>>.Success(allSlots);
    }
}
