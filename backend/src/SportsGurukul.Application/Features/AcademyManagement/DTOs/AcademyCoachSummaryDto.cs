namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class AcademyCoachSummaryDto
{
    public Guid Id { get; set; }
    public Guid CoachId { get; set; }
    public string CoachCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string CoachingLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string? PrimarySport { get; set; }
    public int YearsOfExperience { get; set; }
    public DateTime AssignedDate { get; set; }
}
