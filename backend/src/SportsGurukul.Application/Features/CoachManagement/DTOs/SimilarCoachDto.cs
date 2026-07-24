namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class SimilarCoachDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string CoachCode { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string CoachingLevel { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string? PrimarySport { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public bool IsVerified { get; set; }
    public int MatchScore { get; set; }
}
