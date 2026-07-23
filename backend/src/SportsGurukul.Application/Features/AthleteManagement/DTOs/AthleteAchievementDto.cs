namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class AthleteAchievementDto
{
    public Guid Id { get; set; }
    public Guid AchievementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Competition { get; set; }
    public string? Position { get; set; }
    public string Level { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTime AwardedDate { get; set; }
    public string? Notes { get; set; }
}
