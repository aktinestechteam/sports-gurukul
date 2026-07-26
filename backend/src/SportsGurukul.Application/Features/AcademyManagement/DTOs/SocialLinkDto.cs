namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class SocialLinkDto
{
    public Guid Id { get; set; }
    public Guid AcademyId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
