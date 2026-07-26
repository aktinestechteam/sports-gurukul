namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Advanced academy search request with 40+ filter criteria for academy discovery.
/// Supports geo-radius search, facility filters, and multi-criteria sorting.
/// </summary>
public class AdvancedAcademySearchRequest
{
    /// <summary>Free-text search across name, code, description.</summary>
    /// <example>cricket</example>
    public string? SearchTerm { get; set; }

    /// <summary>Filter by academy name.</summary>
    /// <example>Elite Sports</example>
    public string? Name { get; set; }

    /// <summary>Filter by academy code.</summary>
    /// <example>ACAD-20260725-A1B2</example>
    public string? AcademyCode { get; set; }

    /// <summary>Filter by registration number.</summary>
    /// <example>REG-12345</example>
    public string? RegistrationNumber { get; set; }

    /// <summary>Filter by country.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>Filter by state.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>Filter by city.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>Filter by district.</summary>
    /// <example>Thane</example>
    public string? District { get; set; }

    /// <summary>Filter by PIN code.</summary>
    /// <example>400001</example>
    public string? PinCode { get; set; }

    /// <summary>Latitude for radius search.</summary>
    /// <example>19.0760</example>
    public decimal? Latitude { get; set; }

    /// <summary>Longitude for radius search.</summary>
    /// <example>72.8777</example>
    public decimal? Longitude { get; set; }

    /// <summary>Search radius in kilometers.</summary>
    /// <example>10</example>
    public decimal? RadiusKm { get; set; }

    /// <summary>Filter by sport name.</summary>
    /// <example>Cricket</example>
    public string? SportName { get; set; }

    /// <summary>Filter by sport category.</summary>
    /// <example>Team Sports</example>
    public string? SportCategory { get; set; }

    /// <summary>Has swimming pool.</summary>
    /// <example>true</example>
    public bool? HasSwimmingPool { get; set; }

    /// <summary>Has indoor stadium.</summary>
    /// <example>false</example>
    public bool? HasIndoorStadium { get; set; }

    /// <summary>Has cricket ground.</summary>
    /// <example>true</example>
    public bool? HasCricketGround { get; set; }

    /// <summary>Has football ground.</summary>
    /// <example>false</example>
    public bool? HasFootballGround { get; set; }

    /// <summary>Has gym.</summary>
    /// <example>true</example>
    public bool? HasGym { get; set; }

    /// <summary>Has yoga hall.</summary>
    /// <example>false</example>
    public bool? HasYogaHall { get; set; }

    /// <summary>Has parking.</summary>
    /// <example>true</example>
    public bool? HasParking { get; set; }

    /// <summary>Has medical room.</summary>
    /// <example>false</example>
    public bool? HasMedicalRoom { get; set; }

    /// <summary>Has WiFi.</summary>
    /// <example>true</example>
    public bool? HasWifi { get; set; }

    /// <summary>Has cafeteria.</summary>
    /// <example>false</example>
    public bool? HasCafeteria { get; set; }

    /// <summary>Verified academies only.</summary>
    /// <example>true</example>
    public bool? VerifiedOnly { get; set; }

    /// <summary>Government registered only.</summary>
    /// <example>false</example>
    public bool? GovernmentRegisteredOnly { get; set; }

    /// <summary>Minimum years of establishment.</summary>
    /// <example>5</example>
    public int? MinEstablishmentYears { get; set; }

    /// <summary>Minimum membership price.</summary>
    /// <example>1000</example>
    public decimal? MinMembershipPrice { get; set; }

    /// <summary>Maximum membership price.</summary>
    /// <example>10000</example>
    public decimal? MaxMembershipPrice { get; set; }

    /// <summary>Minimum average rating.</summary>
    /// <example>4.0</example>
    public decimal? MinRating { get; set; }

    /// <summary>Minimum number of coaches.</summary>
    /// <example>3</example>
    public int? MinCoaches { get; set; }

    /// <summary>Minimum number of athletes.</summary>
    /// <example>50</example>
    public int? MinAthletes { get; set; }

    /// <summary>Minimum number of branches.</summary>
    /// <example>1</example>
    public int? MinBranches { get; set; }

    /// <summary>Open right now.</summary>
    /// <example>true</example>
    public bool? OpenNow { get; set; }

    /// <summary>Open on weekends.</summary>
    /// <example>true</example>
    public bool? WeekendOpen { get; set; }

    /// <summary>Sort order: Nearest, HighestRated, MostPopular, Newest, Alphabetical, LowestMembershipCost, MostCoaches.</summary>
    /// <example>Nearest</example>
    public string? SortBy { get; set; }

    /// <summary>Page number (1-based).</summary>
    /// <example>1</example>
    public int Page { get; set; } = 1;

    /// <summary>Items per page (default 20, max 100).</summary>
    /// <example>20</example>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Request for finding nearby academies based on geographic coordinates.
/// </summary>
public class NearbyAcademiesRequest
{
    /// <summary>Latitude of center point.</summary>
    /// <example>19.0760</example>
    public decimal Latitude { get; set; }

    /// <summary>Longitude of center point.</summary>
    /// <example>72.8777</example>
    public decimal Longitude { get; set; }

    /// <summary>Search radius in km (0.1-500, default 10).</summary>
    /// <example>10</example>
    public decimal RadiusKm { get; set; } = 10;

    /// <summary>Maximum results (1-50, default 20).</summary>
    /// <example>20</example>
    public int Limit { get; set; } = 20;

    /// <summary>Filter by sport name.</summary>
    /// <example>Cricket</example>
    public string? SportName { get; set; }
}

/// <summary>
/// Request body for saving an academy search configuration for later use.
/// </summary>
public class SaveAcademySearchRequest
{
    /// <summary>Name for the saved search.</summary>
    /// <example>Mumbai Cricket Academies</example>
    public string SearchName { get; set; } = string.Empty;

    /// <summary>Free-text search term.</summary>
    /// <example>cricket coaching</example>
    public string? SearchTerm { get; set; }

    /// <summary>Filter by city.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>Filter by state.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>Filter by country.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>Filter by district.</summary>
    /// <example>Thane</example>
    public string? District { get; set; }

    /// <summary>Filter by PIN code.</summary>
    /// <example>400001</example>
    public string? PinCode { get; set; }

    /// <summary>Filter by sport name.</summary>
    /// <example>Cricket</example>
    public string? SportName { get; set; }

    /// <summary>Filter by sport category.</summary>
    /// <example>Team Sports</example>
    public string? SportCategory { get; set; }

    /// <summary>Filter by facility type.</summary>
    /// <example>Outdoor</example>
    public string? FacilityType { get; set; }

    /// <summary>Has swimming pool.</summary>
    /// <example>true</example>
    public bool? HasSwimmingPool { get; set; }

    /// <summary>Has indoor stadium.</summary>
    /// <example>false</example>
    public bool? HasIndoorStadium { get; set; }

    /// <summary>Has cricket ground.</summary>
    /// <example>true</example>
    public bool? HasCricketGround { get; set; }

    /// <summary>Has football ground.</summary>
    /// <example>false</example>
    public bool? HasFootballGround { get; set; }

    /// <summary>Has gym.</summary>
    /// <example>true</example>
    public bool? HasGym { get; set; }

    /// <summary>Has yoga hall.</summary>
    /// <example>false</example>
    public bool? HasYogaHall { get; set; }

    /// <summary>Has parking.</summary>
    /// <example>true</example>
    public bool? HasParking { get; set; }

    /// <summary>Has medical room.</summary>
    /// <example>false</example>
    public bool? HasMedicalRoom { get; set; }

    /// <summary>Has WiFi.</summary>
    /// <example>true</example>
    public bool? HasWifi { get; set; }

    /// <summary>Has cafeteria.</summary>
    /// <example>false</example>
    public bool? HasCafeteria { get; set; }

    /// <summary>Verified academies only.</summary>
    /// <example>true</example>
    public bool? VerifiedOnly { get; set; }

    /// <summary>Government registered only.</summary>
    /// <example>false</example>
    public bool? GovernmentRegisteredOnly { get; set; }

    /// <summary>Open right now.</summary>
    /// <example>true</example>
    public bool? OpenNow { get; set; }

    /// <summary>Open on weekends.</summary>
    /// <example>true</example>
    public bool? WeekendOpen { get; set; }

    /// <summary>Minimum membership price.</summary>
    /// <example>1000</example>
    public decimal? MinMembershipPrice { get; set; }

    /// <summary>Maximum membership price.</summary>
    /// <example>10000</example>
    public decimal? MaxMembershipPrice { get; set; }

    /// <summary>Minimum average rating.</summary>
    /// <example>4.0</example>
    public decimal? MinRating { get; set; }

    /// <summary>Number of results at time of save.</summary>
    /// <example>15</example>
    public int ResultCount { get; set; }
}
