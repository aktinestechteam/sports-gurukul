namespace SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

public class EventSearchFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? SportId { get; set; }
    public string? SportName { get; set; }
    public Guid? AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public Guid? CoachId { get; set; }
    public string? CoachName { get; set; }
    public string? SpeakerName { get; set; }
    public string? VenueName { get; set; }
    public string? Location { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public TimeSpan? TimeFrom { get; set; }
    public TimeSpan? TimeTo { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? EventType { get; set; }
    public string? Category { get; set; }
    public string? SkillLevel { get; set; }
    public string? AgeGroup { get; set; }
    public string? Availability { get; set; }
    public string? RegistrationStatus { get; set; }
    public decimal? MinRating { get; set; }
    public string? Language { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? RadiusKm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
