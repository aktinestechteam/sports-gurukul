using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

public class CoachAdvancedSearchResponseExample : IExamplesProvider<ApiResponse<AdvancedCoachSearchResponse>>
{
    public ApiResponse<AdvancedCoachSearchResponse> GetExamples() =>
        ApiResponse<AdvancedCoachSearchResponse>.SuccessResult(
            new AdvancedCoachSearchResponse
            {
                Items = new List<CoachSummaryDto>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        CoachCode = "COACH-20250101-001",
                        FullName = "Rahul Sharma",
                        Email = "rahul@example.com",
                        PhoneNumber = "+919876543210",
                        CoachingLevel = "Senior",
                        Status = "Active",
                        VerificationStatus = "Verified",
                        PrimarySport = "Cricket",
                        SportCategory = "Team Sports",
                        YearsOfExperience = 8,
                        City = "Mumbai",
                        State = "Maharashtra",
                        Country = "India",
                        IsVerified = true,
                        CertificationCount = 3,
                        IsOnlineAvailable = true,
                        IsOfflineAvailable = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-90)
                    }
                },
                TotalRecords = 150,
                TotalPages = 8,
                CurrentPage = 1,
                PageSize = 20,
                NextCursor = "2025-01-01T00:00:00Z"
            },
            "Coaches retrieved successfully.");
}

public class CoachSuggestionsResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<CoachSearchSuggestionDto>>>
{
    public ApiResponse<IReadOnlyList<CoachSearchSuggestionDto>> GetExamples() =>
        ApiResponse<IReadOnlyList<CoachSearchSuggestionDto>>.SuccessResult(
            new List<CoachSearchSuggestionDto>
            {
                new() { Text = "Rahul Sharma", Type = "Name", SubText = "Coach Name" },
                new() { Text = "COACH-20250101-001", Type = "CoachCode", SubText = "Coach Code" }
            },
            "Suggestions retrieved successfully.");
}

public class SimilarCoachesResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<SimilarCoachDto>>>
{
    public ApiResponse<IReadOnlyList<SimilarCoachDto>> GetExamples() =>
        ApiResponse<IReadOnlyList<SimilarCoachDto>>.SuccessResult(
            new List<SimilarCoachDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FullName = "Priya Patel",
                    CoachCode = "COACH-20250215-002",
                    CoachingLevel = "Intermediate",
                    YearsOfExperience = 5,
                    PrimarySport = "Cricket",
                    City = "Pune",
                    State = "Maharashtra",
                    IsVerified = true,
                    MatchScore = 3
                }
            },
            "Similar coaches retrieved successfully.");
}

public class CoachSaveSearchRequestExample : IExamplesProvider<CoachSaveSearchRequest>
{
    public CoachSaveSearchRequest GetExamples() =>
        new()
        {
            Name = "Cricket Coaches in Mumbai",
            FiltersJson = "{\"sportName\":\"Cricket\",\"city\":\"Mumbai\",\"coachingLevel\":\"Senior\"}"
        };
}

public class CoachSavedSearchResponseExample : IExamplesProvider<ApiResponse<SavedSearchDto>>
{
    public ApiResponse<SavedSearchDto> GetExamples() =>
        ApiResponse<SavedSearchDto>.SuccessResult(
            new SavedSearchDto
            {
                Id = Guid.NewGuid(),
                Name = "Cricket Coaches in Mumbai",
                FiltersJson = "{\"sportName\":\"Cricket\",\"city\":\"Mumbai\",\"coachingLevel\":\"Senior\"}",
                UsageCount = 0,
                CreatedAt = DateTime.UtcNow
            },
            "Saved search created successfully.");
}

public class CoachSavedSearchesResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<SavedSearchDto>>>
{
    public ApiResponse<IReadOnlyList<SavedSearchDto>> GetExamples() =>
        ApiResponse<IReadOnlyList<SavedSearchDto>>.SuccessResult(
            new List<SavedSearchDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Cricket Coaches in Mumbai",
                    FiltersJson = "{\"sportName\":\"Cricket\",\"city\":\"Mumbai\"}",
                    UsageCount = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                }
            },
            "Saved searches retrieved successfully.");
}

public class CoachRecentSearchesResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<RecentSearchDto>>>
{
    public ApiResponse<IReadOnlyList<RecentSearchDto>> GetExamples() =>
        ApiResponse<IReadOnlyList<RecentSearchDto>>.SuccessResult(
            new List<RecentSearchDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    QueryText = "Cricket coach Mumbai",
                    FiltersJson = "{\"sportName\":\"Cricket\"}",
                    ResultCount = 12,
                    SearchedAt = DateTime.UtcNow.AddHours(-2)
                }
            },
            "Recent searches retrieved successfully.");
}
