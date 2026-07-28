using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

public class PlatformWaitlistDto
{
    public Guid Id { get; set; }
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public Guid? AthleteId { get; set; }
    public Guid? UserId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int Position { get; set; }
    public WaitlistStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? PromotedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
