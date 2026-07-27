using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class AthleteSportsTests : AthleteIntegrationTestBase
{
    public AthleteSportsTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task AssignSport_ValidRequest_AssignsSportSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SportDto>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Id.Should().Be(SeedData.CricketSportId);
    }

    [Fact]
    public async Task AssignSport_DuplicateSport_ReturnsConflict()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AssignSport_NonExistentSport_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = Guid.NewGuid(),
            IsPrimarySport = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignSport_NonExistentAthlete_ReturnsNotFound()
    {
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{Guid.NewGuid()}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAthleteSports_HasSports_ReturnsSportsList()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete.Id}/sports", new
        {
            SportId = SeedData.FootballSportId,
            IsPrimarySport = false
        });

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/sports");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SportDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAthleteSports_NoSports_ReturnsEmptyList()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/sports");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SportDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAthleteSports_NonExistentAthlete_ReturnsNotFound()
    {
        var response = await AdminClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/sports");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveSport_AssignedSport_RemovesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete.Id}/sports/{SeedData.CricketSportId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var sportsResponse = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/sports");
        var sportsContent = await sportsResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SportDto>>>();
        sportsContent!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveSport_NotAssigned_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}/sports/{SeedData.CricketSportId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignMultipleSports_SetsPrimaryCorrectly()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete.Id}/sports", new
        {
            SportId = SeedData.FootballSportId,
            IsPrimarySport = false
        });

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/sports");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SportDto>>>();
        content!.Data.Should().HaveCount(2);
        content.Data!.Count(s => s.IsPrimarySport).Should().Be(1);
    }
}
