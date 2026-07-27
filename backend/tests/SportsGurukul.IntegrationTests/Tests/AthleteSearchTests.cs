using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class AthleteSearchTests : AthleteIntegrationTestBase
{
    public AthleteSearchTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task SearchAthletes_BasicSearch_ReturnsResults()
    {
        await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync("/api/v1/athletes/search?searchTerm=Test&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteSearchResponse>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Items.Should().NotBeEmpty();
        content.Data.TotalRecords.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SearchAthletes_WithLevelFilter_ReturnsFilteredResults()
    {
        await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync("/api/v1/athletes/search?currentLevel=Intermediate&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteSearchResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().AllSatisfy(a =>
            a.CurrentLevel.Should().Be("Intermediate"));
    }

    [Fact]
    public async Task SearchAthletes_WithPagination_ReturnsCorrectPage()
    {
        await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync("/api/v1/athletes/search?page=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteSearchResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAthletes_NoResults_ReturnsEmptyList()
    {
        var response = await AdminClient.GetAsync("/api/v1/athletes/search?searchTerm=ZZZZNONEXISTENT&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteSearchResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().BeEmpty();
        content.Data.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task SearchAthletes_WithSortBySortDescending_SortsCorrectly()
    {
        await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync("/api/v1/athletes/search?sortBy=name&sortDescending=true&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteSearchResponse>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SearchAthletes_AthleteRole_ReturnsForbidden()
    {
        var response = await AthleteClient.GetAsync("/api/v1/athletes/search");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #region Suggestions

    [Fact]
    public async Task GetSuggestions_WithPrefix_ReturnsMatchingSuggestions()
    {
        await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync("/api/v1/athletes/search/suggestions?prefix=Test&limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSuggestions_NoMatches_ReturnsEmptyList()
    {
        var response = await AdminClient.GetAsync("/api/v1/athletes/search/suggestions?prefix=ZZZZ&limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteSearchSuggestionDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().BeEmpty();
    }

    #endregion

    #region Saved Searches

    [Fact]
    public async Task CreateSavedSearch_ValidRequest_CreatesSuccessfully()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes/search/saved", new
        {
            Name = "My Test Search",
            FiltersJson = "{\"currentLevel\":\"Intermediate\"}"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SavedSearchDto>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Name.Should().Be("My Test Search");
    }

    [Fact]
    public async Task GetSavedSearches_HasSearches_ReturnsList()
    {
        await AdminClient.PostAsJsonAsync("/api/v1/athletes/search/saved", new
        {
            Name = "Search 1",
            FiltersJson = "{}"
        });
        await AdminClient.PostAsJsonAsync("/api/v1/athletes/search/saved", new
        {
            Name = "Search 2",
            FiltersJson = "{}"
        });

        var response = await AdminClient.GetAsync("/api/v1/athletes/search/saved");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SavedSearchDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteSavedSearch_Exists_DeletesSuccessfully()
    {
        var createResponse = await AdminClient.PostAsJsonAsync("/api/v1/athletes/search/saved", new
        {
            Name = "To Delete",
            FiltersJson = "{}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<SavedSearchDto>>();

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/search/saved/{created!.Data!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await AdminClient.GetAsync("/api/v1/athletes/search/saved");
        var getContent = await getResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SavedSearchDto>>>();
        getContent!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteSavedSearch_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/search/saved/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Recent Searches

    [Fact]
    public async Task RecordRecentSearch_ValidRequest_RecordsSuccessfully()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes/search/recent", new
        {
            QueryText = "cricket players",
            FiltersJson = "{}",
            ResultCount = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRecentSearches_HasSearches_ReturnsList()
    {
        await AdminClient.PostAsJsonAsync("/api/v1/athletes/search/recent", new
        {
            QueryText = "query 1",
            FiltersJson = "{}",
            ResultCount = 5
        });
        await AdminClient.PostAsJsonAsync("/api/v1/athletes/search/recent", new
        {
            QueryText = "query 2",
            FiltersJson = "{}",
            ResultCount = 3
        });

        var response = await AdminClient.GetAsync("/api/v1/athletes/search/recent");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<RecentSearchDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentSearches_NoSearches_ReturnsEmptyList()
    {
        var response = await AdminClient.GetAsync("/api/v1/athletes/search/recent");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<RecentSearchDto>>>();
        content!.Data.Should().BeEmpty();
    }

    #endregion
}
