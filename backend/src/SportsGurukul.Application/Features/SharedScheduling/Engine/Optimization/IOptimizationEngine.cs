using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface IOptimizationEngine
{
    Task<TimeSlot?> FindBestAvailableSlotAsync(string resourceType, IReadOnlyList<Guid> resourceIds, DateTime preferredDate, TimeSpan duration, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<Guid?> FindLeastBusyResourceAsync(string resourceType, IReadOnlyList<Guid> resourceIds, DateTime startDate, DateTime endDate, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeSlot>> BalanceCoachLoadAsync(IReadOnlyList<Guid> coachIds, IReadOnlyList<TimeSlot> requestedSlots, SchedulingContext context, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeSlot>> OptimizeResourceAllocationAsync(IReadOnlyList<SchedulingRequest> requests, SchedulingContext context, CancellationToken cancellationToken = default);
}
