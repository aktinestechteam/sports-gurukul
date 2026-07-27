namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record SchedulingRequest
{
    public Guid RequestId { get; init; } = Guid.NewGuid();
    public string RequestType { get; init; } = string.Empty;
    public Guid AcademyId { get; init; }
    public Guid? BranchId { get; init; }
    public TimeSlot TimeSlot { get; init; } = null!;
    public IReadOnlyList<ResourceRequirement> Resources { get; init; } = [];
    public string? Title { get; init; }
    public bool AllowConflicts { get; init; }
    public bool CheckBusinessHours { get; init; } = true;
    public bool CheckHolidays { get; init; } = true;
    public bool SkipMaintenanceWindows { get; init; } = true;
}
