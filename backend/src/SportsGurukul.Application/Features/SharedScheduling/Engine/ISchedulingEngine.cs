using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface ISchedulingEngine
{
    Task<SchedulingResult> ScheduleAsync(SchedulingRequest request, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeSlot>> GenerateOccurrenceSlotsAsync(TimeSlot baseSlot, RecurrencePattern pattern, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<string> GenerateScheduleNumberAsync(string prefix, CancellationToken cancellationToken = default);
    Task<bool> ValidateSlotAsync(TimeSlot slot, SchedulingContext context, IReadOnlyList<ResourceRequirement>? resources = null, CancellationToken cancellationToken = default);
}
