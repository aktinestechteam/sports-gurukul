namespace SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

public class EventCardDto
{
    public Guid Id { get; set; }
    public string EventCode { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? BannerUrl { get; set; }
    public string? EventType { get; set; }
    public string? Category { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int RegistrationCount { get; set; }
    public decimal? RegistrationFee { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublic { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public string? SportName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double? DistanceKm { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int ViewCount { get; set; }
    public int DaysUntilStart { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public bool IsSoldOut { get; set; }
}

public class EventSearchPageResultDto
{
    public IReadOnlyList<EventCardDto> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public string? Cursor { get; set; }
    public double SearchTimeMs { get; set; }
    public IReadOnlyList<string> AvailableSortOptions { get; set; } =
        ["Upcoming", "Popularity", "RecentlyAdded", "HighestRated", "Nearest", "Alphabetical", "RegistrationClosingSoon"];
}

public class EventAutocompleteSuggestionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? SubText { get; set; }
    public string? Highlight { get; set; }
    public string? Category { get; set; }
    public string? EventType { get; set; }
    public DateTime? EventDate { get; set; }
}

public class EventSimilarDto
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string? BannerUrl { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? EventType { get; set; }
    public IReadOnlyList<string> CommonSports { get; set; } = [];
    public IReadOnlyList<string> CommonTags { get; set; } = [];
    public double SimilarityScore { get; set; }
    public DateTime StartDate { get; set; }
    public decimal? RegistrationFee { get; set; }
}
