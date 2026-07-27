using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface IConflictDetectionEngine
{
    Task<IReadOnlyList<ConflictInfo>> DetectConflictsAsync(TimeSlot slot, IReadOnlyList<ResourceRequirement> resources, SchedulingContext context, Guid? excludeItemId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConflictInfo>> DetectConflictsForMultipleSlotsAsync(IReadOnlyList<TimeSlot> slots, IReadOnlyList<ResourceRequirement> resources, SchedulingContext context, Guid? excludeItemId = null, CancellationToken cancellationToken = default);
    Task<bool> HasConflictAsync(TimeSlot slot, IReadOnlyList<ResourceRequirement> resources, SchedulingContext context, Guid? excludeItemId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConflictInfo>> GetUnresolvedConflictsAsync(Guid resourceId, string resourceType, CancellationToken cancellationToken = default);
    Task<bool> ResolveConflictAsync(Guid conflictId, string resolutionNotes, CancellationToken cancellationToken = default);
}
