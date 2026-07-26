namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class MembershipPlanDto
{
    public Guid Id { get; set; }
    public Guid AcademyId { get; set; }
    public string MembershipName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string? Benefits { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
