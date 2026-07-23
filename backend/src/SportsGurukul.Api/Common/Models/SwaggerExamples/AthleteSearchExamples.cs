using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

/// <summary>
/// Swagger response example for advanced search results.
/// </summary>
public class AdvancedSearchResponseExample : IExamplesProvider<ApiResponse<AthleteSearchResponse>>
{
    public ApiResponse<AthleteSearchResponse> GetExamples() => ApiResponse<AthleteSearchResponse>.SuccessResult(
        new AthleteSearchResponse
        {
            Items = new List<AthleteSummaryDto>
            {
                new()
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                    UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                    AthleteCode = "ATH-2026-001",
                    FullName = "Priya Sharma",
                    Email = "priya.sharma@email.com",
                    PhoneNumber = "9876543210",
                    ProfileImageUrl = "https://storage.example.com/profiles/priya.jpg",
                    CurrentLevel = "Intermediate",
                    Status = "Active",
                    PrimarySport = "Cricket",
                    SportCategory = "Team Sports",
                    CurrentRank = "A",
                    StateRank = "5",
                    NationalRank = "120",
                    ExperienceYears = 5,
                    Gender = GenderDto.Female,
                    Age = 24,
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    IsVerified = true,
                    HasMedicalProfile = true,
                    AchievementCount = 8,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                }
            },
            TotalRecords = 1,
            TotalPages = 1,
            CurrentPage = 1,
            PageSize = 20,
            NextCursor = null,
            PreviousCursor = null
        },
        "Athletes retrieved successfully.");
}

/// <summary>
/// Swagger response example for search suggestions.
/// </summary>
public class SuggestionsResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>>>
{
    public ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>> GetExamples() =>
        ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>>.SuccessResult(
            new List<AthleteSearchSuggestionDto>
            {
                new()
                {
                    Text = "Priya Sharma",
                    Type = "athlete",
                    Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                    SubText = "ATH-2026-001"
                },
                new()
                {
                    Text = "Cricket",
                    Type = "sport",
                    SubText = "Bat and ball team sport"
                }
            },
            "Suggestions retrieved successfully.");
}

/// <summary>
/// Swagger response example for saved searches.
/// </summary>
public class SavedSearchesResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<SavedSearchDto>>>
{
    public ApiResponse<IReadOnlyList<SavedSearchDto>> GetExamples() =>
        ApiResponse<IReadOnlyList<SavedSearchDto>>.SuccessResult(
            new List<SavedSearchDto>
            {
                new()
                {
                    Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                    Name = "Mumbai Cricketers",
                    FiltersJson = "{\"city\":\"Mumbai\",\"sportName\":\"Cricket\"}",
                    UsageCount = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                }
            },
            "Saved searches retrieved successfully.");
}

/// <summary>
/// Swagger response example for recent searches.
/// </summary>
public class RecentSearchesResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<RecentSearchDto>>>
{
    public ApiResponse<IReadOnlyList<RecentSearchDto>> GetExamples() =>
        ApiResponse<IReadOnlyList<RecentSearchDto>>.SuccessResult(
            new List<RecentSearchDto>
            {
                new()
                {
                    Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
                    QueryText = "Mumbai cricket",
                    FiltersJson = "{\"city\":\"Mumbai\",\"sportName\":\"Cricket\"}",
                    ResultCount = 15,
                    SearchedAt = DateTime.UtcNow.AddHours(-2)
                }
            },
            "Recent searches retrieved successfully.");
}

/// <summary>
/// Swagger request example for creating a saved search.
/// </summary>
public class CreateSavedSearchRequestExample : IExamplesProvider<CreateSavedSearchRequest>
{
    public CreateSavedSearchRequest GetExamples() => new()
    {
        Name = "Mumbai Intermediate Cricketers",
        FiltersJson = "{\"city\":\"Mumbai\",\"sportName\":\"Cricket\",\"currentLevel\":\"Intermediate\"}"
    };
}

/// <summary>
/// Swagger response example for creating a saved search.
/// </summary>
public class CreateSavedSearchResponseExample : IExamplesProvider<ApiResponse<SavedSearchDto>>
{
    public ApiResponse<SavedSearchDto> GetExamples() => ApiResponse<SavedSearchDto>.SuccessResult(
        new SavedSearchDto
        {
            Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
            Name = "Mumbai Intermediate Cricketers",
            FiltersJson = "{\"city\":\"Mumbai\",\"sportName\":\"Cricket\",\"currentLevel\":\"Intermediate\"}",
            UsageCount = 0,
            CreatedAt = DateTime.UtcNow
        },
        "Saved search created successfully.");
}
