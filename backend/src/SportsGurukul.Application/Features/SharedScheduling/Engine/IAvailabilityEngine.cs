using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface IAvailabilityEngine
{
    Task<IReadOnlyList<TimeSlot>> GetAvailableSlotsAsync(Guid resourceId, string resourceType, DateTime date, SchedulingContext context, TimeSpan? slotDuration = null, CancellationToken cancellationToken = default);
    Task<TimeSlot?> GetNextAvailableSlotAsync(Guid resourceId, string resourceType, DateTime fromDate, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeSlot>> GetAlternativeSlotsAsync(Guid resourceId, string resourceType, TimeSlot requestedSlot, int maxAlternatives, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<AvailabilityWindow> GetAvailabilityWindowAsync(Guid resourceId, string resourceType, DateTime date, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UtilizationMetric>> GetResourceUtilizationAsync(string resourceType, IReadOnlyList<Guid> resourceIds, DateTime startDate, DateTime endDate, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeakHourInfo>> GetPeakHoursAsync(Guid resourceId, string resourceType, DateTime startDate, DateTime endDate, SchedulingContext context, CancellationToken cancellationToken = default);
}
