namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class AcademySummaryDto
{
    public Guid Id { get; set; }
    public string AcademyCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public int TotalBranches { get; set; }
    public int TotalFacilities { get; set; }
    public int TotalSports { get; set; }
    public int TotalMemberships { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}
