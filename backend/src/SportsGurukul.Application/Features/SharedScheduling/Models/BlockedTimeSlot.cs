namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public enum BlockedReason { Maintenance = 0, CoachLeave = 1, FacilityClosure = 2, EventOverride = 3, AdminBlock = 4 }

public sealed record BlockedTimeSlot
{
    public Guid ResourceId { get; init; }
    public string ResourceType { get; init; } = string.Empty;
    public TimeSlot Slot { get; init; } = null!;
    public BlockedReason Reason { get; init; }
    public string? Notes { get; init; }
}
