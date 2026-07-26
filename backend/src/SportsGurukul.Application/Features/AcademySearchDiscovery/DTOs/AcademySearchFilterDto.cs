namespace SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

public class AcademySearchFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? AcademyCode { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PinCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? RadiusKm { get; set; }
    public string? SportName { get; set; }
    public string? SportCategory { get; set; }
    public bool? HasSwimmingPool { get; set; }
    public bool? HasIndoorStadium { get; set; }
    public bool? HasCricketGround { get; set; }
    public bool? HasFootballGround { get; set; }
    public bool? HasGym { get; set; }
    public bool? HasYogaHall { get; set; }
    public bool? HasParking { get; set; }
    public bool? HasMedicalRoom { get; set; }
    public bool? HasWifi { get; set; }
    public bool? HasCafeteria { get; set; }
    public bool? VerifiedOnly { get; set; }
    public bool? GovernmentRegisteredOnly { get; set; }
    public int? MinEstablishmentYears { get; set; }
    public decimal? MinMembershipPrice { get; set; }
    public decimal? MaxMembershipPrice { get; set; }
    public decimal? MinRating { get; set; }
    public int? MinCoaches { get; set; }
    public int? MinAthletes { get; set; }
    public int? MinBranches { get; set; }
    public bool? OpenNow { get; set; }
    public bool? WeekendOpen { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class AcademySearchResultDto
{
    public Guid Id { get; set; }
    public string AcademyCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsGovernmentRegistered { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public int YearsEstablished { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PinCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double? DistanceKm { get; set; }
    public IReadOnlyList<string> Sports { get; set; } = [];
    public IReadOnlyList<string> FacilityTypes { get; set; } = [];
    public IReadOnlyList<string> FacilityNames { get; set; } = [];
    public decimal? MinMembershipPrice { get; set; }
    public decimal? MaxMembershipPrice { get; set; }
    public int TotalCoaches { get; set; }
    public int TotalAthletes { get; set; }
    public int TotalBranches { get; set; }
    public int TotalFacilities { get; set; }
    public int TotalMemberships { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public int ViewCount { get; set; }
    public bool IsOpenNow { get; set; }
    public bool IsWeekendOpen { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AcademySearchPageResultDto
{
    public IReadOnlyList<AcademySearchResultDto> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public string? Cursor { get; set; }
    public IReadOnlyList<string> AvailableSortOptions { get; set; } = 
        ["Nearest", "HighestRated", "MostPopular", "Newest", "Alphabetical", "LowestMembershipCost", "MostCoaches"];
}

public class AcademySuggestionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AcademyCode { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsVerified { get; set; }
}

public class AcademySimilarDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AcademyCode { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public bool IsVerified { get; set; }
    public IReadOnlyList<string> CommonSports { get; set; } = [];
    public IReadOnlyList<string> CommonFacilities { get; set; } = [];
    public double SimilarityScore { get; set; }
}

public class SavedAcademySearchDto
{
    public Guid Id { get; set; }
    public string SearchName { get; set; } = string.Empty;
    public string? SearchTerm { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? SportName { get; set; }
    public string? FacilityType { get; set; }
    public bool? VerifiedOnly { get; set; }
    public decimal? MinMembershipPrice { get; set; }
    public decimal? MaxMembershipPrice { get; set; }
    public int ResultCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecentAcademySearchDto
{
    public Guid Id { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? SportName { get; set; }
    public int AcademyCount { get; set; }
    public DateTime SearchedAt { get; set; }
}

public class PopularAcademyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AcademyCode { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public bool IsVerified { get; set; }
    public int ViewCount { get; set; }
    public decimal AverageRating { get; set; }
}
