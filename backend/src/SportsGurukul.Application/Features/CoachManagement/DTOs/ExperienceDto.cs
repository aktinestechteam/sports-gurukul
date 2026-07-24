namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class ExperienceDto
{
    public Guid Id { get; set; }
    public string Organization { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Sport { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
