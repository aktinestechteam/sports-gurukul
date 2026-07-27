namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record AvailabilityWindow
{
    public Guid ResourceId { get; init; }
    public string ResourceType { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public IReadOnlyList<TimeSlot> AvailableSlots { get; init; } = [];
    public IReadOnlyList<TimeSlot> BlockedSlots { get; init; } = [];
    public decimal UtilizationPercent { get; init; }
    public bool IsFullyBooked => AvailableSlots.Count == 0;
}
