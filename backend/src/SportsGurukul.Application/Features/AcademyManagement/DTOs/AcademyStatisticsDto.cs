namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

public class AcademyStatisticsDto
{
    public Guid AcademyId { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public int TotalCoaches { get; set; }
    public int TotalAthletes { get; set; }
    public int TotalBranches { get; set; }
    public int TotalFacilities { get; set; }
    public int ActiveMemberships { get; set; }
    public int SportsOffered { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalGalleryImages { get; set; }
}
