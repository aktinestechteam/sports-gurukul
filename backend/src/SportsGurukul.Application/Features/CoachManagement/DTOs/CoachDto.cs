namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CoachCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string? Biography { get; set; }
    public int YearsOfExperience { get; set; }
    public string? CurrentOrganization { get; set; }
    public string? HighestQualification { get; set; }
    public string? PreferredLanguage { get; set; }
    public string CoachingLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public IReadOnlyList<SportDto> Sports { get; set; } = [];
    public IReadOnlyList<CertificationDto> Certifications { get; set; } = [];
    public IReadOnlyList<ExperienceDto> Experiences { get; set; } = [];
    public IReadOnlyList<EducationDto> Education { get; set; } = [];
    public AvailabilityDto? Availability { get; set; }
    public LocationDto? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
