namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Competition { get; set; }
    public string? Position { get; set; }
    public string Level { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
