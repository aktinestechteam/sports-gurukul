namespace SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

public class SavedEventSearchDto
{
    public Guid Id { get; set; }
    public string SearchName { get; set; } = string.Empty;
    public string? SearchTerm { get; set; }
    public string? SportName { get; set; }
    public string? AcademyName { get; set; }
    public string? CoachName { get; set; }
    public string? SpeakerName { get; set; }
    public string? VenueName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? EventType { get; set; }
    public string? Category { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SkillLevel { get; set; }
    public string? AgeGroup { get; set; }
    public string? Language { get; set; }
    public string? SortBy { get; set; }
    public int ResultCount { get; set; }
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecentEventSearchDto
{
    public Guid Id { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string? SportName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? EventType { get; set; }
    public int ResultCount { get; set; }
    public DateTime SearchedAt { get; set; }
}
