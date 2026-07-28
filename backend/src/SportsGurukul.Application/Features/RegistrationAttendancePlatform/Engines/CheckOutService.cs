using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class CheckOutService : ICheckOutService
{
    private readonly ILogger<CheckOutService> _logger;

    public CheckOutService(ILogger<CheckOutService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> CanCheckOutAsync(Guid participantId, Guid? sessionId, Func<Guid, Guid?, CancellationToken, Task<DateTime?>> getCheckInTime, CancellationToken cancellationToken = default)
    {
        var checkInTime = await getCheckInTime(participantId, sessionId, cancellationToken);
        if (!checkInTime.HasValue)
        {
            _logger.LogWarning("Check-out rejected: participant {ParticipantId} has not checked in", participantId);
            return false;
        }

        _logger.LogInformation("Check-out allowed for participant {ParticipantId}", participantId);
        return true;
    }

    public Task<TimeSpan?> CalculateDurationAsync(DateTime checkInTime, DateTime checkOutTime)
    {
        var duration = checkOutTime - checkInTime;
        _logger.LogInformation("Calculated duration: {Duration} (Check-in: {CheckInTime}, Check-out: {CheckOutTime})", duration, checkInTime, checkOutTime);
        return Task.FromResult<TimeSpan?>(duration);
    }

    public Task<bool> IsMinimumDurationMetAsync(DateTime checkInTime, DateTime checkOutTime, TimeSpan minimumDuration)
    {
        var duration = checkOutTime - checkInTime;
        var met = duration >= minimumDuration;
        _logger.LogInformation("Minimum duration check: {Met} (actual: {Actual}, required: {Required})", met, duration, minimumDuration);
        return Task.FromResult(met);
    }
}
