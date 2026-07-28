namespace SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

public class NearbyEventDto
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? BannerUrl { get; set; }
    public string? EventType { get; set; }
    public DateTime StartDate { get; set; }
    public decimal? RegistrationFee { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double DistanceKm { get; set; }
    public decimal AverageRating { get; set; }
    public int ViewCount { get; set; }
    public bool IsRegistrationOpen { get; set; }
}
