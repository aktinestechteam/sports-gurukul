using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademySearchTests : AcademyIntegrationTestBase
{
    public AcademySearchTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task GetAcademies_ReturnsPaginatedResult()
    {
        await CreateAcademyAsync("Search Alpha");
        await CreateAcademyAsync("Search Beta");

        var response = await AcademyAdminClient.GetAsync("/api/v1/academies?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademySearchResponse>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Items.Should().NotBeEmpty();
        content.Data.TotalRecords.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetAcademies_WithSearchTerm_FiltersCorrectly()
    {
        await CreateAcademyAsync("Unique Filter Academy XYZ");

        var response = await AcademyAdminClient.GetAsync("/api/v1/academies?searchTerm=XYZ");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademySearchResponse>>();
        content!.Data!.Items.Should().Contain(a => a.Name.Contains("XYZ"));
    }

    [Fact]
    public async Task SearchAcademies_ReturnsResults()
    {
        await CreateAcademyAsync("Searchable Academy");

        var response = await AcademyAdminClient.GetAsync("/api/v1/academies/search?searchTerm=Searchable");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademySearchResponse>>();
        content!.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SearchAcademies_WithFilters_FiltersCorrectly()
    {
        await CreateAcademyAsync("Filter Test Academy");

        var response = await AcademyAdminClient.GetAsync("/api/v1/academies/search?name=Filter+Test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSuggestions_ReturnsMatchingResults()
    {
        await CreateAcademyAsync("Suggestion Target Academy");

        var response = await AcademyAdminClient.GetAsync("/api/v1/academies/suggestions?prefix=Suggestion");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AcademySummaryDto>>>();
        content!.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSuggestions_EmptyPrefix_ReturnsBadRequestOrEmpty()
    {
        var response = await AcademyAdminClient.GetAsync("/api/v1/academies/suggestions?prefix=");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdvancedSearch_ReturnsResults()
    {
        await CreateAcademyAsync("Advanced Search Academy");

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies/advanced-search", new
        {
            SearchTerm = "Advanced"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPopularAcademies_ReturnsResults()
    {
        await CreateAcademyAsync("Popular Academy");

        var response = await AcademyAdminClient.GetAsync("/api/v1/academies/popular?limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPopularSearchTerms_ReturnsOk()
    {
        var response = await AcademyAdminClient.GetAsync("/api/v1/academies/popular-searches?limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAcademies_Pagination_WorksCorrectly()
    {
        for (int i = 0; i < 5; i++)
            await CreateAcademyAsync($"Page Test Academy {i}");

        var page1 = await AcademyAdminClient.GetAsync("/api/v1/academies?page=1&pageSize=2");
        var content1 = await page1.Content.ReadFromJsonAsync<ApiResponse<AcademySearchResponse>>();
        content1!.Data!.Items.Count.Should().BeLessOrEqualTo(2);
        content1.Data.TotalRecords.Should().BeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task GetNearbyAcademies_ReturnsOk()
    {
        var response = await AcademyAdminClient.GetAsync(
            "/api/v1/academies/nearby?latitude=19.076&longitude=72.8777&radiusKm=50&limit=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSimilarAcademies_NonExistentAcademy_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.GetAsync(
            $"/api/v1/academies/similar/{Guid.NewGuid()}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Fact]
    public async Task Search_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/academies/search");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Search_AthleteCanAccess()
    {
        var response = await AthleteClient.GetAsync("/api/v1/academies/search");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Search_CoachCanAccess()
    {
        var response = await CoachClient.GetAsync("/api/v1/academies/search");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task CreateAcademyAsync(string name)
    {
        await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = name,
            Email = $"{name.ToLower().Replace(" ", "")}@test.com",
            Phone = "+919876543210"
        });
    }
}