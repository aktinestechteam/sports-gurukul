using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public interface ISchedulingEngine
{
    Task<string> GenerateBookingNumberAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingSchedule>> GenerateScheduleInstancesAsync(
        Booking booking,
        DateTime startDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int? occurrenceCount,
        DateTime? endDate,
        CancellationToken cancellationToken = default);
    Task<bool> IsSlotAvailableAsync(
        Guid academyId,
        Guid? facilityId,
        Guid? coachId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default);
}
