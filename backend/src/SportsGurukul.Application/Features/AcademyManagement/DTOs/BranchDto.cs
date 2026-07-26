namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class BranchDto
{
    public Guid Id { get; set; }
    public Guid AcademyId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
