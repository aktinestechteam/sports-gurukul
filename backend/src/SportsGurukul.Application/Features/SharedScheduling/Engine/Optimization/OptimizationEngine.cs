using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class OptimizationEngine : IOptimizationEngine
{
    private readonly IAvailabilityEngine _availabilityEngine;
    private readonly ILogger<OptimizationEngine> _logger;

    public OptimizationEngine(IAvailabilityEngine availabilityEngine, ILogger<OptimizationEngine> logger)
    {
        _availabilityEngine = availabilityEngine;
        _logger = logger;
    }

    public async Task<TimeSlot?> FindBestAvailableSlotAsync(
        string resourceType, IReadOnlyList<Guid> resourceIds, DateTime preferredDate,
        TimeSpan duration, SchedulingContext context, CancellationToken cancellationToken = default)
    {
        for (int d = 0; d < 14; d++)
        {
            var date = preferredDate.Date.AddDays(d);
            foreach (var resourceId in resourceIds)
            {
                var slots = await _availabilityEngine.GetAvailableSlotsAsync(
                    resourceId, resourceType, date, context, duration, cancellationToken);

                var match = slots.FirstOrDefault(s => s.DurationMinutes >= (int)duration.TotalMinutes);
                if (match is not null)
                {
                    _logger.LogInformation("Best slot found: {Date} {Start}-{End} for {ResourceType}:{ResourceId}",
                        match.Date.Date, match.StartTime, match.EndTime, resourceType, resourceId);
                    return match;
                }
            }
        }

        _logger.LogInformation("No best slot found within 14 days for {ResourceType}", resourceType);
        return null;
    }

    public async Task<Guid?> FindLeastBusyResourceAsync(
        string resourceType, IReadOnlyList<Guid> resourceIds, DateTime startDate, DateTime endDate,
        SchedulingContext context, CancellationToken cancellationToken = default)
    {
        var metrics = await _availabilityEngine.GetResourceUtilizationAsync(
            resourceType, resourceIds, startDate, endDate, context, cancellationToken);

        var leastBusy = metrics.OrderBy(m => m.UtilizationPercent).FirstOrDefault();
        if (leastBusy is null) return null;

        _logger.LogInformation("Least busy {ResourceType}: {ResourceId} at {Utilization}%",
            resourceType, leastBusy.ResourceId, leastBusy.UtilizationPercent);
        return leastBusy.ResourceId;
    }

    public async Task<IReadOnlyList<TimeSlot>> BalanceCoachLoadAsync(
        IReadOnlyList<Guid> coachIds, IReadOnlyList<TimeSlot> requestedSlots,
        SchedulingContext context, CancellationToken cancellationToken = default)
    {
        if (coachIds.Count == 0 || requestedSlots.Count == 0) return [];

        var balanced = new List<TimeSlot>();
        var coachSlots = coachIds.ToDictionary(c => c, _ => 0);

        foreach (var slot in requestedSlots)
        {
            var leastLoaded = coachSlots.OrderBy(kv => kv.Value).First();

            var available = await _availabilityEngine.GetAvailableSlotsAsync(
                leastLoaded.Key, "Coach", slot.Date, context, cancellationToken: cancellationToken);

            if (available.Any(s => s.Overlaps(slot)))
            {
                balanced.Add(slot);
                coachSlots[leastLoaded.Key]++;
            }
        }

        _logger.LogInformation("Balanced {Count} slots across {CoachCount} coaches", balanced.Count, coachIds.Count);
        return balanced;
    }

    public async Task<IReadOnlyList<TimeSlot>> OptimizeResourceAllocationAsync(
        IReadOnlyList<SchedulingRequest> requests, SchedulingContext context, CancellationToken cancellationToken = default)
    {
        var optimized = new List<TimeSlot>();

        foreach (var request in requests.OrderBy(r => r.TimeSlot.Date))
        {
            var result = new SchedulingEngine(null!, null!, null!, null!, null!, null!);
            var bestSlot = await FindBestAvailableSlotAsync(
                request.Resources.FirstOrDefault()?.ResourceType ?? "Unknown",
                request.Resources.Select(r => r.ResourceId).ToList(),
                request.TimeSlot.Date,
                request.TimeSlot.EndTime - request.TimeSlot.StartTime,
                context, cancellationToken);

            if (bestSlot is not null)
                optimized.Add(bestSlot);
        }

        _logger.LogInformation("Optimized {Count} resource allocations", optimized.Count);
        return optimized;
    }
}
