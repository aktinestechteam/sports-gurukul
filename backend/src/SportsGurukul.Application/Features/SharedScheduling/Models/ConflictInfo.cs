namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public enum ConflictSeverity { Low = 0, Medium = 1, High = 2, Critical = 3 }
public enum ConflictType { TimeOverlap = 0, ResourceExhaustion = 1, BusinessHoursViolation = 2, HolidayConflict = 3, MaintenanceWindow = 4 }

public sealed record ConflictInfo
{
    public Guid ConflictId { get; init; } = Guid.NewGuid();
    public ConflictType Type { get; init; }
    public ConflictSeverity Severity { get; init; }
    public string ResourceType { get; init; } = string.Empty;
    public Guid ResourceId { get; init; }
    public Guid? ConflictingItemId { get; init; }
    public string? Description { get; init; }
    public TimeSlot OverlappingSlot { get; init; } = null!;
    public IReadOnlyList<TimeSlot> SuggestedAlternatives { get; init; } = [];
}
