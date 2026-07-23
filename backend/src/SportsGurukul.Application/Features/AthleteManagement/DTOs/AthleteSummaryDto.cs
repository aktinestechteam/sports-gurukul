namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class AthleteSummaryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AthleteCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PrimarySport { get; set; }
    public string? CurrentRank { get; set; }
    public int ExperienceYears { get; set; }
    public DateTime CreatedAt { get; set; }
}
