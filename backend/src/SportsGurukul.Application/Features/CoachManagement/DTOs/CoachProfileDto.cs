namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachProfileDto
{
    public CoachDto Coach { get; set; } = null!;
    public IReadOnlyList<SportDto> Sports { get; set; } = [];
    public IReadOnlyList<CertificationDto> Certifications { get; set; } = [];
    public IReadOnlyList<ExperienceDto> Experiences { get; set; } = [];
    public IReadOnlyList<EducationDto> Education { get; set; } = [];
    public AvailabilityDto? Availability { get; set; }
    public LocationDto? Location { get; set; }
    public IReadOnlyList<AssignedAthleteDto> AssignedAthletes { get; set; } = [];
}
