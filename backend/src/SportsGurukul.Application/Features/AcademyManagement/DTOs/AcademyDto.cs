namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class AcademyDto
{
    public Guid Id { get; set; }
    public string AcademyCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Description { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? GSTNumber { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public string? Website { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public ContactDto? Contact { get; set; }
    public OperatingHoursDto? OperatingHours { get; set; }
    public IReadOnlyList<BranchDto> Branches { get; set; } = [];
    public IReadOnlyList<AcademySportDto> Sports { get; set; } = [];
    public IReadOnlyList<FacilityDto> Facilities { get; set; } = [];
    public IReadOnlyList<MembershipPlanDto> Memberships { get; set; } = [];
    public IReadOnlyList<SocialLinkDto> SocialLinks { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
