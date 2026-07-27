namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record BusinessHours
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeSpan OpenTime { get; init; }
    public TimeSpan CloseTime { get; init; }
    public bool IsClosed { get; init; }
    public bool IsMaintenanceWindow { get; init; }
    public string? Notes { get; init; }
    
    public bool Contains(TimeSpan time) => !IsClosed && time >= OpenTime && time < CloseTime;
    public bool ContainsSlot(TimeSlot slot) => !IsClosed && !IsMaintenanceWindow && Contains(slot.StartTime) && slot.EndTime <= CloseTime;
}
