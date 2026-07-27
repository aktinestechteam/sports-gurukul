using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class AvailabilityEngine : IAvailabilityEngine
{
    private readonly ITimeSlotGenerator _slotGenerator;
    private readonly IBusinessHoursProvider _businessHoursProvider;
    private readonly ILogger<AvailabilityEngine> _logger;

    public AvailabilityEngine(
        ITimeSlotGenerator slotGenerator,
        IBusinessHoursProvider businessHoursProvider,
        ILogger<AvailabilityEngine> logger)
    {
        _slotGenerator = slotGenerator;
        _businessHoursProvider = businessHoursProvider;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TimeSlot>> GetAvailableSlotsAsync(
        Guid resourceId, string resourceType, DateTime date,
        SchedulingContext context, TimeSpan? slotDuration = null,
        CancellationToken cancellationToken = default)
    {
        var duration = slotDuration ?? context.SlotDuration;
        var businessHourSlots = await _businessHoursProvider.GetBusinessHourSlotsAsync(
            resourceId, resourceType, date, duration, cancellationToken);

        var blockedSlots = context.BlockedSlots
            .Where(b => b.ResourceId == resourceId && b.ResourceType == resourceType &&
                        b.Slot.Date.Date == date.Date)
            .Select(b => b.Slot)
            .ToList();

        var available = _slotGenerator.SubtractSlots(businessHourSlots, blockedSlots);

        _logger.LogDebug(
            "Resource {ResourceType}:{ResourceId} on {Date}: {Count} available slots",
            resourceType, resourceId, date.Date, available.Count);

        return available;
    }

    public async Task<TimeSlot?> GetNextAvailableSlotAsync(
        Guid resourceId, string resourceType, DateTime fromDate,
        SchedulingContext context, CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < 30; i++)
        {
            var date = fromDate.Date.AddDays(i);
            var slots = await GetAvailableSlotsAsync(
                resourceId, resourceType, date, context, cancellationToken: cancellationToken);

            if (slots.Count > 0)
                return slots[0];
        }

        return null;
    }

    public async Task<IReadOnlyList<TimeSlot>> GetAlternativeSlotsAsync(
        Guid resourceId, string resourceType, TimeSlot requestedSlot,
        int maxAlternatives, SchedulingContext context,
        CancellationToken cancellationToken = default)
    {
        var alternatives = new List<TimeSlot>();

        for (int i = 0; i < 30 && alternatives.Count < maxAlternatives; i++)
        {
            var date = requestedSlot.Date.Date.AddDays(i);
            var slots = await GetAvailableSlotsAsync(
                resourceId, resourceType, date, context, cancellationToken: cancellationToken);

            var matching = slots.Where(s => s.DurationMinutes >= requestedSlot.DurationMinutes).ToList();
            alternatives.AddRange(matching.Take(maxAlternatives - alternatives.Count));
        }

        return alternatives.Take(maxAlternatives).ToList();
    }

    public async Task<AvailabilityWindow> GetAvailabilityWindowAsync(
        Guid resourceId, string resourceType, DateTime date,
        SchedulingContext context, CancellationToken cancellationToken = default)
    {
        var available = await GetAvailableSlotsAsync(
            resourceId, resourceType, date, context, cancellationToken: cancellationToken);

        var blocked = context.BlockedSlots
            .Where(b => b.ResourceId == resourceId && b.ResourceType == resourceType &&
                        b.Slot.Date.Date == date.Date)
            .Select(b => b.Slot)
            .ToList();

        var totalSlots = available.Count + blocked.Count;
        var utilization = totalSlots > 0
            ? Math.Round((decimal)blocked.Count / totalSlots * 100, 2)
            : 0;

        return new AvailabilityWindow
        {
            ResourceId = resourceId,
            ResourceType = resourceType,
            Date = date,
            AvailableSlots = available,
            BlockedSlots = blocked,
            UtilizationPercent = utilization
        };
    }

    public async Task<IReadOnlyList<UtilizationMetric>> GetResourceUtilizationAsync(
        string resourceType, IReadOnlyList<Guid> resourceIds,
        DateTime startDate, DateTime endDate, SchedulingContext context,
        CancellationToken cancellationToken = default)
    {
        var metrics = new List<UtilizationMetric>();

        foreach (var resourceId in resourceIds)
        {
            var totalSlots = 0;
            var bookedSlots = 0;
            var peakHours = new List<PeakHourInfo>();
            var current = startDate.Date;

            while (current <= endDate.Date)
            {
                var available = await GetAvailableSlotsAsync(
                    resourceId, resourceType, current, context, cancellationToken: cancellationToken);

                totalSlots += available.Count + context.BlockedSlots
                    .Count(b => b.ResourceId == resourceId && b.ResourceType == resourceType &&
                                b.Slot.Date.Date == current.Date);

                var dayBlocked = context.BlockedSlots
                    .Count(b => b.ResourceId == resourceId && b.ResourceType == resourceType &&
                                b.Slot.Date.Date == current.Date);
                bookedSlots += dayBlocked;

                current = current.AddDays(1);
            }

            metrics.Add(new UtilizationMetric
            {
                ResourceId = resourceId,
                ResourceType = resourceType,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalSlots = totalSlots,
                BookedSlots = bookedSlots,
                PeakHours = peakHours
            });
        }

        return metrics;
    }

    public Task<IReadOnlyList<PeakHourInfo>> GetPeakHoursAsync(
        Guid resourceId, string resourceType, DateTime startDate,
        DateTime endDate, SchedulingContext context,
        CancellationToken cancellationToken = default)
    {
        var hours = Enumerable.Range(6, 16)
            .Select(h => new PeakHourInfo { Hour = h, BookingCount = 0 })
            .ToList();

        return Task.FromResult<IReadOnlyList<PeakHourInfo>>(hours);
    }
}
