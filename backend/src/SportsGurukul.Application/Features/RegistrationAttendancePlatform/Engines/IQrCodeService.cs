using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public interface IQrCodeService
{
    Task<string> GenerateQrCodeDataAsync(QrCodeType type, ProgramType programType, Guid programId, Guid participantId, CancellationToken cancellationToken = default);
    Task<bool> ValidateQrCodeAsync(string qrCodeData, QrCodeType expectedType, CancellationToken cancellationToken = default);
    Task<DateTime?> GetExpirationAsync(QrCodeType type, ProgramType programType);
    string EncodePayload(QrCodeType type, ProgramType programType, Guid programId, Guid participantId);
}
