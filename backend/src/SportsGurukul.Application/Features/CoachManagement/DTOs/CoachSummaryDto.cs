namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachSummaryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CoachCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string CoachingLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string? PrimarySport { get; set; }
    public string? SportCategory { get; set; }
    public int YearsOfExperience { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public bool IsVerified { get; set; }
    public int CertificationCount { get; set; }
    public bool IsOnlineAvailable { get; set; }
    public bool IsOfflineAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}
