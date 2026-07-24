using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

public class CoachAdvancedSearchRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? CoachCode { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? SportName { get; set; }
    public Guid[]? SportIds { get; set; }
    public string? SportCategory { get; set; }
    public CoachingLevel? CoachingLevel { get; set; }
    public int? MinExperience { get; set; }
    public int? MaxExperience { get; set; }
    public string? CertificationName { get; set; }
    public VerificationStatus? CertificationStatus { get; set; }
    public string? CurrentOrganization { get; set; }
    public string? HighestQualification { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public bool? AvailableToday { get; set; }
    public bool? OnlineAvailable { get; set; }
    public bool? OfflineAvailable { get; set; }
    public bool? IsVerified { get; set; }
    public bool? BackgroundVerified { get; set; }
    public string? Language { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    public string? Cursor { get; set; }
    public bool UseCursorPagination { get; set; }
}

public class CoachSaveSearchRequest
{
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
}

public class CoachRecordRecentSearchRequest
{
    public string QueryText { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
    public int ResultCount { get; set; }
}
