namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class AssignedAthleteDto
{
    public Guid Id { get; set; }
    public Guid AthleteId { get; set; }
    public string AthleteCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CurrentLevel { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PrimarySport { get; set; }
    public DateTime AssignedDate { get; set; }
}
