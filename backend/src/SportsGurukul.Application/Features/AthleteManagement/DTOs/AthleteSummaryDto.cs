namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class AthleteSummaryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AthleteCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PrimarySport { get; set; }
    public string? SportCategory { get; set; }
    public string? CurrentRank { get; set; }
    public string? StateRank { get; set; }
    public string? NationalRank { get; set; }
    public string? InternationalRank { get; set; }
    public int ExperienceYears { get; set; }
    public GenderDto? Gender { get; set; }
    public int? Age { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public bool IsVerified { get; set; }
    public bool HasMedicalProfile { get; set; }
    public int AchievementCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum GenderDto
{
    Male = 0,
    Female = 1,
    NonBinary = 2,
    PreferNotToSay = 3
}
