using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;

public class QrCodeService : IQrCodeService
{
    private readonly ILogger<QrCodeService> _logger;

    public QrCodeService(ILogger<QrCodeService> logger)
    {
        _logger = logger;
    }

    public Task<string> GenerateQrCodeDataAsync(QrCodeType type, ProgramType programType, Guid programId, Guid participantId, CancellationToken cancellationToken = default)
    {
        var payload = EncodePayload(type, programType, programId, participantId);
        var hash = ComputeHash(payload);
        var qrData = $"SG-{type}-{hash}";
        _logger.LogInformation("Generated QR code data: {QrData} for participant {ParticipantId}", qrData, participantId);
        return Task.FromResult(qrData);
    }

    public Task<bool> ValidateQrCodeAsync(string qrCodeData, QrCodeType expectedType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(qrCodeData))
        {
            return Task.FromResult(false);
        }

        var isValid = qrCodeData.StartsWith($"SG-{expectedType}-");
        if (!isValid)
        {
            _logger.LogWarning("QR code validation failed: expected type {ExpectedType}, got data {QrData}", expectedType, qrCodeData);
        }
        return Task.FromResult(isValid);
    }

    public Task<DateTime?> GetExpirationAsync(QrCodeType type, ProgramType programType)
    {
        DateTime? expiration = type switch
        {
            QrCodeType.Registration => DateTime.UtcNow.AddDays(30),
            QrCodeType.Attendance => DateTime.UtcNow.AddHours(24),
            QrCodeType.Certificate => null,
            _ => DateTime.UtcNow.AddHours(24)
        };

        _logger.LogInformation("QR code expiration for type {Type}: {Expiration}", type, expiration);
        return Task.FromResult(expiration);
    }

    public string EncodePayload(QrCodeType type, ProgramType programType, Guid programId, Guid participantId)
    {
        return $"{type}:{programType}:{programId}:{participantId}:{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes[..16]).ToLowerInvariant();
    }
}
