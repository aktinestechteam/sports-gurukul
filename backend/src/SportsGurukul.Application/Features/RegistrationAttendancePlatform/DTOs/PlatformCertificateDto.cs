using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

public class PlatformCertificateDto
{
    public Guid Id { get; set; }
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public CertificateType CertificateType { get; set; }
    public PlatformCertificateStatus Status { get; set; }
    public DateTime IssuedDate { get; set; }
    public string? IssuedBy { get; set; }
    public string? DocumentUrl { get; set; }
    public string? TemplateId { get; set; }
    public bool IsPrinted { get; set; }
    public bool IsSent { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
