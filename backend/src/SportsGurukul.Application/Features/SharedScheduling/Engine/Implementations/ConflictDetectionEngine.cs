using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class ConflictDetectionEngine : IConflictDetectionEngine
{
    private readonly ILogger<ConflictDetectionEngine> _logger;
    private readonly List<ConflictInfo> _conflictStore = [];

    public ConflictDetectionEngine(ILogger<ConflictDetectionEngine> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<ConflictInfo>> DetectConflictsAsync(
        TimeSlot slot, IReadOnlyList<ResourceRequirement> resources,
        SchedulingContext context, Guid? excludeItemId = null,
        CancellationToken cancellationToken = default)
    {
        var conflicts = new List<ConflictInfo>();

        if (context.CheckBusinessHours && !slot.Date.Equals(default))
        {
            var dayStart = context.DayStartTime.ToTimeSpan();
            var dayEnd = context.DayEndTime.ToTimeSpan();
            if (slot.StartTime < dayStart || slot.EndTime > dayEnd)
            {
                conflicts.Add(new ConflictInfo
                {
                    Type = ConflictType.BusinessHoursViolation,
                    Severity = ConflictSeverity.Medium,
                    ResourceType = "System",
                    ResourceId = Guid.Empty,
                    Description = $"Slot {slot.StartTime}-{slot.EndTime} is outside business hours ({dayStart}-{dayEnd})",
                    OverlappingSlot = slot
                });
            }
        }

        if (context.Holidays.Any(h => h.Date.Date == slot.Date.Date))
        {
            conflicts.Add(new ConflictInfo
            {
                Type = ConflictType.HolidayConflict,
                Severity = ConflictSeverity.High,
                ResourceType = "System",
                ResourceId = Guid.Empty,
                Description = $"Date {slot.Date:yyyy-MM-dd} is a holiday",
                OverlappingSlot = slot
            });
        }

        foreach (var resource in resources)
        {
            var blocked = context.BlockedSlots
                .FirstOrDefault(b => b.ResourceId == resource.ResourceId &&
                                     b.ResourceType == resource.ResourceType &&
                                     b.Slot.Overlaps(slot));

            if (blocked is not null)
            {
                conflicts.Add(new ConflictInfo
                {
                    Type = ConflictType.MaintenanceWindow,
                    Severity = ConflictSeverity.High,
                    ResourceType = resource.ResourceType,
                    ResourceId = resource.ResourceId,
                    Description = $"Resource blocked: {blocked.Reason}",
                    OverlappingSlot = blocked.Slot
                });
            }
        }

        _logger.LogDebug("Detected {Count} conflicts for slot {Date} {Start}-{End}",
            conflicts.Count, slot.Date, slot.StartTime, slot.EndTime);

        _conflictStore.AddRange(conflicts);
        return Task.FromResult<IReadOnlyList<ConflictInfo>>(conflicts);
    }

    public Task<IReadOnlyList<ConflictInfo>> DetectConflictsForMultipleSlotsAsync(
        IReadOnlyList<TimeSlot> slots, IReadOnlyList<ResourceRequirement> resources,
        SchedulingContext context, Guid? excludeItemId = null,
        CancellationToken cancellationToken = default)
    {
        var allConflicts = new List<ConflictInfo>();
        foreach (var slot in slots)
        {
            var conflicts = DetectConflictsAsync(
                slot, resources, context, excludeItemId, cancellationToken).Result;
            allConflicts.AddRange(conflicts);
        }
        return Task.FromResult<IReadOnlyList<ConflictInfo>>(allConflicts);
    }

    public Task<bool> HasConflictAsync(
        TimeSlot slot, IReadOnlyList<ResourceRequirement> resources,
        SchedulingContext context, Guid? excludeItemId = null,
        CancellationToken cancellationToken = default)
    {
        var conflicts = DetectConflictsAsync(
            slot, resources, context, excludeItemId, cancellationToken).Result;
        return Task.FromResult(conflicts.Count > 0);
    }

    public Task<IReadOnlyList<ConflictInfo>> GetUnresolvedConflictsAsync(
        Guid resourceId, string resourceType, CancellationToken cancellationToken = default)
    {
        var unresolved = _conflictStore
            .Where(c => c.ResourceId == resourceId && c.ResourceType == resourceType)
            .ToList();

        return Task.FromResult<IReadOnlyList<ConflictInfo>>(unresolved);
    }

    public Task<bool> ResolveConflictAsync(
        Guid conflictId, string resolutionNotes, CancellationToken cancellationToken = default)
    {
        var conflict = _conflictStore.FirstOrDefault(c => c.ConflictId == conflictId);
        if (conflict is null)
        {
            _logger.LogWarning("Conflict {ConflictId} not found", conflictId);
            return Task.FromResult(false);
        }

        _conflictStore.Remove(conflict);
        _logger.LogInformation("Resolved conflict {ConflictId}: {Notes}", conflictId, resolutionNotes);
        return Task.FromResult(true);
    }
}