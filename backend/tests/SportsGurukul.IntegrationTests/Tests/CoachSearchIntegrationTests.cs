using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class CoachSearchIntegrationTests : CoachIntegrationTestBase
{
    public CoachSearchIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task SearchCoaches_ReturnsResults()
    {
        await CreateTestCoachAsync();

        var response = await UnauthenticatedClient.GetAsync("/api/v1/coaches/search?SearchTerm=cricket&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AdvancedCoachSearchResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchCoaches_WithSportFilter_ReturnsResults()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coaches/search?SportName=Cricket&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AdvancedCoachSearchResponse>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchCoaches_WithLocationFilter_ReturnsResults()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync("/api/v1/coaches/search?City=Mumbai&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchCoaches_EmptyQuery_ReturnsResults()
    {
        await CreateTestCoachAsync();

        var response = await UnauthenticatedClient.GetAsync("/api/v1/coaches/search?Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchCoaches_Pagination_ReturnsCorrectPage()
    {
        for (int i = 0; i < 5; i++)
        {
            await CreateTestCoachAsync();
        }

        var response = await UnauthenticatedClient.GetAsync("/api/v1/coaches/search?Page=1&PageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AdvancedCoachSearchResponse>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCoachSuggestions_ReturnsSuggestions()
    {
        await CreateTestCoachAsync();

        var response = await UnauthenticatedClient.GetAsync("/api/v1/coaches/search/suggestions?prefix=coa");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<CoachSearchSuggestionDto>>>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCoachSuggestions_EmptyPrefix_ReturnsOk()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/coaches/search/suggestions?prefix=");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<CoachSearchSuggestionDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSimilarCoaches_ExistingCoach_ReturnsResults()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coaches/search/similar/{coach!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SimilarCoachDto>>>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSimilarCoaches_NonExistentCoach_ReturnsOk()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coaches/search/similar/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveSearch_Admin_SavesSuccessfully()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/coaches/search/saved", new CoachSaveSearchRequest
        {
            Name = "Cricket Coaches in Mumbai",
            FiltersJson = "{\"sport\":\"cricket\",\"city\":\"Mumbai\"}"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetSavedSearches_Admin_ReturnsList()
    {
        await AdminClient.PostAsJsonAsync("/api/v1/coaches/search/saved", new CoachSaveSearchRequest
        {
            Name = "Saved Search",
            FiltersJson = "{}"
        });

        var response = await AdminClient.GetAsync("/api/v1/coaches/search/saved");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SavedSearchDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteSavedSearch_Admin_DeletesSuccessfully()
    {
        var saveResponse = await AdminClient.PostAsJsonAsync("/api/v1/coaches/search/saved", new CoachSaveSearchRequest
        {
            Name = "To Delete",
            FiltersJson = "{}"
        });
        var saveContent = await saveResponse.Content.ReadFromJsonAsync<ApiResponse<SavedSearchDto>>();
        var searchId = saveContent!.Data!.Id;

        var response = await AdminClient.DeleteAsync($"/api/v1/coaches/search/saved/{searchId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TrackRecentSearch_Admin_TracksSuccessfully()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/coaches/search/recent", new CoachRecordRecentSearchRequest
        {
            QueryText = "cricket coaches",
            FiltersJson = "{}",
            ResultCount = 5
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetRecentSearches_Admin_ReturnsList()
    {
        await AdminClient.PostAsJsonAsync("/api/v1/coaches/search/recent", new CoachRecordRecentSearchRequest
        {
            QueryText = "cricket",
            FiltersJson = "{}",
            ResultCount = 3
        });

        var response = await AdminClient.GetAsync("/api/v1/coaches/search/recent");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchCoaches_Authenticated_Works()
    {
        await CreateTestCoachAsync();

        var response = await CoachClient.GetAsync("/api/v1/coaches/search?SearchTerm=test&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}