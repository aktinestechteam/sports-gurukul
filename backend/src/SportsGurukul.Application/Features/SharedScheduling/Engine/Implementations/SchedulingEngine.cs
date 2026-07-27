using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class SchedulingEngine : ISchedulingEngine
{
    private readonly IAvailabilityEngine _availabilityEngine;
    private readonly IConflictDetectionEngine _conflictDetectionEngine;
    private readonly IRecurrenceEngine _recurrenceEngine;
    private readonly ITimeSlotGenerator _slotGenerator;
    private readonly IBusinessHoursProvider _businessHoursProvider;
    private readonly ILogger<SchedulingEngine> _logger;

    public SchedulingEngine(
        IAvailabilityEngine availabilityEngine,
        IConflictDetectionEngine conflictDetectionEngine,
        IRecurrenceEngine recurrenceEngine,
        ITimeSlotGenerator slotGenerator,
        IBusinessHoursProvider businessHoursProvider,
        ILogger<SchedulingEngine> logger)
    {
        _availabilityEngine = availabilityEngine;
        _conflictDetectionEngine = conflictDetectionEngine;
        _recurrenceEngine = recurrenceEngine;
        _slotGenerator = slotGenerator;
        _businessHoursProvider = businessHoursProvider;
        _logger = logger;
    }

    public async Task<SchedulingResult> ScheduleAsync(SchedulingRequest request, SchedulingContext context, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        if (request.CheckHolidays && await IsHolidayAsync(request.TimeSlot.Date, context, cancellationToken))
        {
            sw.Stop();
            return SchedulingResult.Failure($"Date {request.TimeSlot.Date:yyyy-MM-dd} is a holiday", computation: sw.Elapsed);
        }

        var conflicts = await _conflictDetectionEngine.DetectConflictsAsync(
            request.TimeSlot, request.Resources, context, request.RequestId, cancellationToken);

        if (conflicts.Count > 0 && !request.AllowConflicts)
        {
            sw.Stop();
            var alternatives = await GetAlternativesForAllResourcesAsync(request, context, cancellationToken);
            return SchedulingResult.Failure(
                $"Detected {conflicts.Count} conflict(s)",
                conflicts,
                sw.Elapsed) with { Alternatives = alternatives };
        }

        sw.Stop();
        _logger.LogInformation(
            "Successfully scheduled {RequestType} for {Date} {Start}-{End} in {Elapsed}ms",
            request.RequestType, request.TimeSlot.Date.Date, request.TimeSlot.StartTime, request.TimeSlot.EndTime, sw.ElapsedMilliseconds);

        return SchedulingResult.Success([request.TimeSlot], sw.Elapsed);
    }

    public async Task<IReadOnlyList<TimeSlot>> GenerateOccurrenceSlotsAsync(
        TimeSlot baseSlot, RecurrencePattern pattern, SchedulingContext context,
        CancellationToken cancellationToken = default)
    {
        var occurrenceDates = _recurrenceEngine.GenerateOccurrences(pattern, baseSlot.Date);

        if (pattern.SkipHolidays)
        {
            occurrenceDates = _recurrenceEngine.FilterOccurrences(occurrenceDates, context);
        }

        var slots = occurrenceDates.Select(date => new TimeSlot
        {
            Date = date.Date,
            StartTime = baseSlot.StartTime,
            EndTime = baseSlot.EndTime
        }).ToList();

        _logger.LogInformation(
            "Generated {Count} occurrence slots from pattern {Frequency}",
            slots.Count, pattern.Frequency);

        return slots;
    }

    public Task<string> GenerateScheduleNumberAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return Task.FromResult($"{prefix}-{datePart}-{randomPart}");
    }

    public async Task<bool> ValidateSlotAsync(
        TimeSlot slot, SchedulingContext context,
        IReadOnlyList<ResourceRequirement>? resources = null, CancellationToken cancellationToken = default)
    {
        if (context.CheckBusinessHours && await _businessHoursProvider.IsWithinBusinessHoursAsync(Guid.Empty, "System", slot, cancellationToken) == false)
        {
            return false;
        }

        if (resources is not null && resources.Count > 0)
        {
            var conflicts = await _conflictDetectionEngine.DetectConflictsAsync(slot, resources, context, cancellationToken: cancellationToken);
            return conflicts.Count == 0;
        }

        return true;
    }

    private async Task<bool> IsHolidayAsync(DateTime date, SchedulingContext context, CancellationToken cancellationToken)
    {
        return context.Holidays.Any(h => h.Date.Date == date.Date);
    }

    private async Task<IReadOnlyList<TimeSlot>> GetAlternativesForAllResourcesAsync(
        SchedulingRequest request, SchedulingContext context, CancellationToken cancellationToken)
    {
        var allAlternatives = new List<TimeSlot>();
        foreach (var resource in request.Resources)
        {
            var alts = await _availabilityEngine.GetAlternativeSlotsAsync(
                resource.ResourceId, resource.ResourceType, request.TimeSlot, 3, context, cancellationToken);
            allAlternatives.AddRange(alts);
        }
        return _slotGenerator.MergeSlots(allAlternatives.Distinct().ToList());
    }
}
