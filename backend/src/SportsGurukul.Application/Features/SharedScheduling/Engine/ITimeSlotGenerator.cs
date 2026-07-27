using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface ITimeSlotGenerator
{
    IReadOnlyList<TimeSlot> GenerateDailySlots(DateTime date, TimeOnly startTime, TimeOnly endTime, TimeSpan slotDuration, TimeSpan? buffer = null);
    IReadOnlyList<TimeSlot> GenerateSlotsForDateRange(DateTime startDate, DateTime endDate, TimeOnly startTime, TimeOnly endTime, TimeSpan slotDuration, TimeSpan? buffer = null);
    IReadOnlyList<TimeSlot> GenerateSlotsExcluding(DateTime date, TimeOnly startTime, TimeOnly endTime, TimeSpan slotDuration, IReadOnlyList<TimeSlot> existingSlots, TimeSpan? buffer = null);
    IReadOnlyList<TimeSlot> MergeSlots(IReadOnlyList<TimeSlot> slots);
    IReadOnlyList<TimeSlot> SubtractSlots(IReadOnlyList<TimeSlot> available, IReadOnlyList<TimeSlot> blocked);
}
