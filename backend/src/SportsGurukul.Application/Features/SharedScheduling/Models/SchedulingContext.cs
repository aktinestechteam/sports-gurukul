namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record SchedulingContext
{
    public Guid AcademyId { get; init; }
    public Guid? BranchId { get; init; }
    public string TimeZoneId { get; init; } = "UTC";
    public IReadOnlyList<BusinessHours> BusinessHours { get; init; } = [];
    public IReadOnlyList<Holiday> Holidays { get; init; } = [];
    public IReadOnlyList<BlockedTimeSlot> BlockedSlots { get; init; } = [];
    public TimeSpan SlotDuration { get; init; } = TimeSpan.FromMinutes(30);
    public TimeSpan SlotBuffer { get; init; } = TimeSpan.FromMinutes(0);
    public TimeOnly DayStartTime { get; init; } = new(6, 0);
    public TimeOnly DayEndTime { get; init; } = new(22, 0);
    public bool CheckBusinessHours { get; init; } = true;
    public bool CheckHolidays { get; init; } = true;
}
