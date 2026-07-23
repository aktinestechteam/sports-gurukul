namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class AthleteDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AthleteCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? BloodGroup { get; set; }
    public string? DominantHand { get; set; }
    public string? DominantFoot { get; set; }
    public string? Biography { get; set; }
    public string Status { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = [];
    public IReadOnlyList<SportDto> Sports { get; set; } = [];
    public IReadOnlyList<AthleteAchievementDto> Achievements { get; set; } = [];
    public MedicalProfileDto? MedicalProfile { get; set; }
    public EmergencyContactDto? EmergencyContact { get; set; }
    public RankingDto? Ranking { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
