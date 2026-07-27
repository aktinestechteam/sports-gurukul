namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public interface IAvailabilityService
{
    Task<bool> IsFacilityAvailableAsync(
        Guid facilityId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default);
    Task<bool> IsCoachAvailableAsync(
        Guid coachId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default);
    Task<bool> IsAthleteAvailableAsync(
        Guid athleteId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default);
}
