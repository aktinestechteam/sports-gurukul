using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

public class PlatformAttendanceDto
{
    public Guid Id { get; set; }
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public Guid? SessionId { get; set; }
    public string? SessionTitle { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public PlatformAttendanceStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Method { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}
