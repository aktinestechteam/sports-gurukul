using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class AttendanceEngine : IAttendanceEngine
{
    private readonly ILogger<AttendanceEngine> _logger;

    public AttendanceEngine(ILogger<AttendanceEngine> logger)
    {
        _logger = logger;
    }

    public async Task<bool> CanCheckInAsync(Guid participantId, Func<Guid, CancellationToken, Task<bool>> isActiveRegistration, CancellationToken cancellationToken = default)
    {
        var isActive = await isActiveRegistration(participantId, cancellationToken);
        if (!isActive)
        {
            _logger.LogWarning("Check-in rejected: participant {ParticipantId} does not have an active registration", participantId);
        }
        return isActive;
    }

    public async Task<bool> CanCheckOutAsync(Guid participantId, Func<Guid, CancellationToken, Task<bool>> isCheckedIn, CancellationToken cancellationToken = default)
    {
        var checkedIn = await isCheckedIn(participantId, cancellationToken);
        if (!checkedIn)
        {
            _logger.LogWarning("Check-out rejected: participant {ParticipantId} is not checked in", participantId);
        }
        return checkedIn;
    }

    public Task<double> CalculateAttendanceRateAsync(Guid programId, int totalParticipants, int presentCount)
    {
        if (totalParticipants == 0) return Task.FromResult(0.0);
        var rate = Math.Round((double)presentCount / totalParticipants * 100, 2);
        _logger.LogInformation("Attendance rate for program {ProgramId}: {Rate}% ({Present}/{Total})", programId, rate, presentCount, totalParticipants);
        return Task.FromResult(rate);
    }

    public Task<PlatformAttendanceStatus> DetermineAttendanceStatusAsync(DateTime checkInTime, DateTime scheduledStartTime, DateTime? checkOutTime, DateTime scheduledEndTime)
    {
        var lateThreshold = TimeSpan.FromMinutes(15);
        var earlyDepartureThreshold = TimeSpan.FromMinutes(15);

        PlatformAttendanceStatus status;

        if (checkInTime > scheduledStartTime + lateThreshold)
        {
            status = PlatformAttendanceStatus.Late;
        }
        else
        {
            status = PlatformAttendanceStatus.Present;
        }

        if (checkOutTime.HasValue)
        {
            var duration = checkOutTime.Value - checkInTime;
            var totalDuration = scheduledEndTime - scheduledStartTime;

            if (duration < totalDuration * 0.5)
            {
                status = PlatformAttendanceStatus.Partial;
            }
            else if (checkOutTime.Value < scheduledEndTime - earlyDepartureThreshold)
            {
                status = PlatformAttendanceStatus.Late;
            }
        }

        _logger.LogInformation("Determined attendance status: {Status} for check-in at {CheckInTime}", status, checkInTime);
        return Task.FromResult(status);
    }
}
