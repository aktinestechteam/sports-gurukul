using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class CheckInService : ICheckInService
{
    private readonly ILogger<CheckInService> _logger;

    public CheckInService(ILogger<CheckInService> logger)
    {
        _logger = logger;
    }

    public async Task<Guid?> ValidateQrCodeForCheckInAsync(string qrCodeData, Func<string, CancellationToken, Task<Guid?>> validateQr, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(qrCodeData))
        {
            _logger.LogWarning("QR code validation failed: empty QR code data");
            return null;
        }

        var participantId = await validateQr(qrCodeData, cancellationToken);
        if (participantId == null)
        {
            _logger.LogWarning("QR code validation failed: invalid or expired QR code");
        }
        else
        {
            _logger.LogInformation("QR code validated for participant {ParticipantId}", participantId);
        }

        return participantId;
    }

    public async Task<bool> IsAlreadyCheckedInAsync(Guid participantId, Guid? sessionId, Func<Guid, Guid?, CancellationToken, Task<bool>> checkExists, CancellationToken cancellationToken = default)
    {
        var exists = await checkExists(participantId, sessionId, cancellationToken);
        if (exists)
        {
            _logger.LogWarning("Participant {ParticipantId} already checked in for session {SessionId}", participantId, sessionId);
        }
        return exists;
    }

    public string GetCheckInMethod(bool isQrScan, bool isManual, bool isGeofence)
    {
        return (isQrScan, isManual, isGeofence) switch
        {
            (true, _, _) => "QRScan",
            (_, true, _) => "Manual",
            (_, _, true) => "Geofence",
            _ => "Manual"
        };
    }
}
