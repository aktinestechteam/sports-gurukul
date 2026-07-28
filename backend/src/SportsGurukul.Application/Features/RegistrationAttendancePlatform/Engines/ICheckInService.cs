using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface ICheckInService
{
    Task<Guid?> ValidateQrCodeForCheckInAsync(string qrCodeData, Func<string, CancellationToken, Task<Guid?>> validateQr, CancellationToken cancellationToken = default);
    Task<bool> IsAlreadyCheckedInAsync(Guid participantId, Guid? sessionId, Func<Guid, Guid?, CancellationToken, Task<bool>> checkExists, CancellationToken cancellationToken = default);
    string GetCheckInMethod(bool isQrScan, bool isManual, bool isGeofence);
}
