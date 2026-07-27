using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface IBusinessHoursProvider
{
    Task<IReadOnlyList<BusinessHours>> GetBusinessHoursAsync(Guid resourceId, string resourceType, CancellationToken cancellationToken = default);
    Task<bool> IsWithinBusinessHoursAsync(Guid resourceId, string resourceType, TimeSlot slot, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeSlot>> GetBusinessHourSlotsAsync(Guid resourceId, string resourceType, DateTime date, TimeSpan slotDuration, CancellationToken cancellationToken = default);
    Task<bool> IsMaintenanceWindowAsync(Guid resourceId, string resourceType, DateTime date, CancellationToken cancellationToken = default);
}
