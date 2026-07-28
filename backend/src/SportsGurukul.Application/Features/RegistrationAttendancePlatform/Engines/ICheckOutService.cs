namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface ICheckOutService
{
    Task<bool> CanCheckOutAsync(Guid participantId, Guid? sessionId, Func<Guid, Guid?, CancellationToken, Task<DateTime?>> getCheckInTime, CancellationToken cancellationToken = default);
    Task<TimeSpan?> CalculateDurationAsync(DateTime checkInTime, DateTime checkOutTime);
    Task<bool> IsMinimumDurationMetAsync(DateTime checkInTime, DateTime checkOutTime, TimeSpan minimumDuration);
}
