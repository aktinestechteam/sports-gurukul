namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class FacilityDto
{
    public Guid Id { get; set; }
    public Guid AcademyId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty;
    public string? IndoorOutdoor { get; set; }
    public int? Capacity { get; set; }
    public bool Available { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
