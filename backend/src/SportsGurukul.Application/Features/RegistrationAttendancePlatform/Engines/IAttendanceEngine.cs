using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface IAttendanceEngine
{
    Task<bool> CanCheckInAsync(Guid participantId, Func<Guid, CancellationToken, Task<bool>> isActiveRegistration, CancellationToken cancellationToken = default);
    Task<bool> CanCheckOutAsync(Guid participantId, Func<Guid, CancellationToken, Task<bool>> isCheckedIn, CancellationToken cancellationToken = default);
    Task<double> CalculateAttendanceRateAsync(Guid programId, int totalParticipants, int presentCount);
    Task<PlatformAttendanceStatus> DetermineAttendanceStatusAsync(DateTime checkInTime, DateTime scheduledStartTime, DateTime? checkOutTime, DateTime scheduledEndTime);
}
