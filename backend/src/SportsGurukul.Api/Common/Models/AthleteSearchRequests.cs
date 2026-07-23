using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Advanced search request with 30+ filter criteria for athlete discovery.
/// Supports both offset-based (Page/PageSize) and cursor-based pagination.
/// </summary>
public class AdvancedSearchRequest
{
    /// <summary>Free-text search across name, athlete code, and email.</summary>
    public string? SearchTerm { get; set; }

    /// <summary>Filter by athlete name (partial match).</summary>
    public string? Name { get; set; }

    /// <summary>Filter by athlete code (partial match).</summary>
    public string? AthleteCode { get; set; }

    /// <summary>Filter by email (partial match).</summary>
    public string? Email { get; set; }

    /// <summary>Filter by mobile number (partial match).</summary>
    public string? Mobile { get; set; }

    /// <summary>Filter by sport name (partial match).</summary>
    public string? SportName { get; set; }

    /// <summary>Filter by sport category (partial match).</summary>
    public string? SportCategory { get; set; }

    /// <summary>Filter by primary sport flag.</summary>
    public bool? IsPrimarySport { get; set; }

    /// <summary>Filter by specific sport IDs (max 20).</summary>
    public List<Guid>? SportIds { get; set; }

    /// <summary>Filter by city (partial match).</summary>
    public string? City { get; set; }

    /// <summary>Filter by state (partial match).</summary>
    public string? State { get; set; }

    /// <summary>Filter by country (partial match).</summary>
    public string? Country { get; set; }

    /// <summary>Filter by district (partial match).</summary>
    public string? District { get; set; }

    /// <summary>Filter by postal code (partial match).</summary>
    public string? PostalCode { get; set; }

    /// <summary>Filter by current skill level.</summary>
    public AthleteLevel? CurrentLevel { get; set; }

    /// <summary>Filter by ranking text (partial match across all ranking fields).</summary>
    public string? Ranking { get; set; }

    /// <summary>Filter by state rank (partial match).</summary>
    public string? StateRank { get; set; }

    /// <summary>Filter by national rank (partial match).</summary>
    public string? NationalRank { get; set; }

    /// <summary>Filter by international rank (partial match).</summary>
    public string? InternationalRank { get; set; }

    /// <summary>Filter by gender.</summary>
    public Gender? Gender { get; set; }

    /// <summary>Minimum age filter.</summary>
    public int? MinAge { get; set; }

    /// <summary>Maximum age filter.</summary>
    public int? MaxAge { get; set; }

    /// <summary>Filter by minimum height (exact match).</summary>
    public string? MinHeight { get; set; }

    /// <summary>Filter by maximum height (exact match).</summary>
    public string? MaxHeight { get; set; }

    /// <summary>Filter by minimum weight (exact match).</summary>
    public string? MinWeight { get; set; }

    /// <summary>Filter by maximum weight (exact match).</summary>
    public string? MaxWeight { get; set; }

    /// <summary>Filter by blood group.</summary>
    public BloodGroup? BloodGroup { get; set; }

    /// <summary>Minimum years of experience.</summary>
    public int? MinExperience { get; set; }

    /// <summary>Maximum years of experience.</summary>
    public int? MaxExperience { get; set; }

    /// <summary>Filter by athlete status.</summary>
    public AthleteStatus? Status { get; set; }

    /// <summary>Filter by email verification status.</summary>
    public bool? IsVerified { get; set; }

    /// <summary>Filter by whether athlete has a medical profile.</summary>
    public bool? HasMedicalProfile { get; set; }

    /// <summary>Filter by minimum achievement level.</summary>
    public AchievementLevel? MinAchievementLevel { get; set; }

    /// <summary>Filter by created date (from).</summary>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>Filter by created date (to).</summary>
    public DateTime? CreatedTo { get; set; }

    /// <summary>Sort field: name, athletecode, level, experience, ranking, achievementcount, recentlyupdated, newest, oldest.</summary>
    public string? SortBy { get; set; }

    /// <summary>When true, sorts in descending order.</summary>
    public bool SortDescending { get; set; }

    /// <summary>Page number (1-based, default 1).</summary>
    public int Page { get; set; } = 1;

    /// <summary>Items per page (default 20, max 100).</summary>
    public int PageSize { get; set; } = 20;

    /// <summary>Cursor for cursor-based pagination.</summary>
    public string? Cursor { get; set; }

    /// <summary>Use cursor-based pagination instead of offset.</summary>
    public bool UseCursorPagination { get; set; }
}

/// <summary>
/// Request body for creating a saved search configuration.
/// </summary>
public class CreateSavedSearchRequest
{
    /// <summary>Name for the saved search.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>JSON-serialized filter criteria.</summary>
    public string FiltersJson { get; set; } = "{}";
}
