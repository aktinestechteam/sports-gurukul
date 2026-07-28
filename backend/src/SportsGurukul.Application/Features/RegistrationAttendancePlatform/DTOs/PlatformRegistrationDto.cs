using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

public class PlatformRegistrationDto
{
    public Guid Id { get; set; }
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public Guid? AthleteId { get; set; }
    public Guid? UserId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public PlatformRegistrationStatus Status { get; set; }
    public decimal? AmountPaid { get; set; }
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public int? WaitlistPosition { get; set; }
    public string? QrCodeData { get; set; }
    public DateTime CreatedAt { get; set; }
}
