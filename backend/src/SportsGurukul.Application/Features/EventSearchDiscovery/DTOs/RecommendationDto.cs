namespace SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

public class RecommendationDto
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
    public decimal AverageRating { get; set; }
    public double RelevanceScore { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public string? MatchedTags { get; set; }
}

public class PersonalizedRecommendationDto
{
    public IReadOnlyList<RecommendationDto> ForYou { get; set; } = [];
    public IReadOnlyList<RecommendationDto> BasedOnHistory { get; set; } = [];
    public IReadOnlyList<RecommendationDto> PopularInYourArea { get; set; } = [];
    public IReadOnlyList<RecommendationDto> NewAndNoteworthy { get; set; } = [];
}

public class TrendingEventDto
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string? BannerUrl { get; set; }
    public string? EventType { get; set; }
    public DateTime StartDate { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int RegistrationCount { get; set; }
    public decimal AverageRating { get; set; }
    public int TrendingScore { get; set; }
}

public class FeaturedEventDto
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? BannerUrl { get; set; }
    public string? EventType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? RegistrationFee { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public string? SportName { get; set; }
    public decimal AverageRating { get; set; }
    public int RegistrationCount { get; set; }
    public int? MaxParticipants { get; set; }
    public int Priority { get; set; }
}
