using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.OptimizeSchedule;

public class OptimizeScheduleCommandHandler
    : IRequestHandler<OptimizeScheduleCommand, Result<TimeSlot?>>
{
    private readonly IOptimizationEngine _optimizationEngine;
    private readonly ILogger<OptimizeScheduleCommandHandler> _logger;

    public OptimizeScheduleCommandHandler(
        IOptimizationEngine optimizationEngine,
        ILogger<OptimizeScheduleCommandHandler> logger)
    {
        _optimizationEngine = optimizationEngine;
        _logger = logger;
    }

    public async Task<Result<TimeSlot?>> Handle(
        OptimizeScheduleCommand request, CancellationToken cancellationToken)
    {
        var context = new SchedulingContext
        {
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            TimeZoneId = request.TimeZoneId ?? "UTC"
        };

        var bestSlot = await _optimizationEngine.FindBestAvailableSlotAsync(
            request.ResourceType, request.ResourceIds, request.PreferredDate,
            request.Duration, context, cancellationToken);

        if (bestSlot is null)
        {
            _logger.LogWarning(
                "No optimal slot found for {ResourceType} on {Date}",
                request.ResourceType, request.PreferredDate);
            return Result<TimeSlot?>.Success(null);
        }

        _logger.LogInformation(
            "Optimal slot found: {Date} {Start}-{End}",
            bestSlot.Date, bestSlot.StartTime, bestSlot.EndTime);

        return Result<TimeSlot?>.Success(bestSlot);
    }
}
