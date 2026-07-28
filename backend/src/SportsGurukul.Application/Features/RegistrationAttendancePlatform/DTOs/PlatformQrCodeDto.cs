using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

public class PlatformQrCodeDto
{
    public Guid Id { get; set; }
    public QrCodeType Type { get; set; }
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public Guid? ParticipantId { get; set; }
    public string QrCodeData { get; set; } = string.Empty;
    public string? EncodedPayload { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsValid { get; set; }
}
