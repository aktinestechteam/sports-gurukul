using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AuthorizationTests : AthleteIntegrationTestBase
{
    public AuthorizationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateAthlete_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = Guid.NewGuid(),
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAthlete_AthleteRole_Returns403()
    {
        var response = await AthleteClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = Guid.NewGuid(),
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAthlete_CoachRole_Returns403()
    {
        var response = await CoachClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = Guid.NewGuid(),
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAthlete_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.DeleteAsync($"/api/v1/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteAthlete_CoachRole_Returns403()
    {
        var response = await CoachClient.DeleteAsync($"/api/v1/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAthlete_AthleteRole_Returns403()
    {
        var response = await AthleteClient.DeleteAsync($"/api/v1/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RestoreAthlete_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsync($"/api/v1/athletes/{Guid.NewGuid()}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RestoreAthlete_CoachRole_Returns403()
    {
        var response = await CoachClient.PostAsync($"/api/v1/athletes/{Guid.NewGuid()}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAthletes_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAthletes_AthleteRole_Returns403()
    {
        var response = await AthleteClient.GetAsync("/api/v1/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Search_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/athletes/search");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Search_AthleteRole_Returns403()
    {
        var response = await AthleteClient.GetAsync("/api/v1/athletes/search");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSuggestions_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/athletes/search/suggestions?prefix=test");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AssignSport_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/athletes/{Guid.NewGuid()}/sports", new
        {
            SportId = Guid.NewGuid(),
            IsPrimarySport = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAthleteById_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMedicalProfile_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PutAsJsonAsync($"/api/v1/athletes/{Guid.NewGuid()}/medical-profile", new
        {
            BloodGroup = "O+"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateEmergencyContact_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PutAsJsonAsync($"/api/v1/athletes/{Guid.NewGuid()}/emergency-contact", new
        {
            Name = "Test",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateRanking_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PutAsJsonAsync($"/api/v1/athletes/{Guid.NewGuid()}/ranking", new
        {
            CurrentRank = "1"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMedicalProfile_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/medical-profile");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetEmergencyContact_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/emergency-contact");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRanking_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/ranking");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAchievements_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/achievements");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSports_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/sports");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminRole_CanAccessAllEndpoints()
    {
        var athlete = await CreateTestAthleteAsync();

        var endpoints = new Func<Task<HttpResponseMessage>>[]
        {
            () => AdminClient.GetAsync("/api/v1/athletes?page=1&pageSize=5"),
            () => AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}"),
            () => AdminClient.GetAsync($"/api/v1/athletes/user/{SeedData.AthleteUserId}"),
            () => AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/sports"),
            () => AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/achievements"),
            () => AdminClient.GetAsync($"/api/v1/athletes/search?searchTerm=test"),
            () => AdminClient.GetAsync("/api/v1/athletes/search/suggestions?prefix=test"),
            () => AdminClient.GetAsync("/api/v1/athletes/search/saved"),
            () => AdminClient.GetAsync("/api/v1/athletes/search/recent")
        };

        foreach (var endpoint in endpoints)
        {
            var response = await endpoint();
            response.StatusCode.Should().Be(HttpStatusCode.OK,
                because: $"Admin should access {response.RequestMessage!.RequestUri}");
        }
    }

    [Fact]
    public async Task CoachRole_CanAccessAthleteListAndSearch()
    {
        var athlete = await CreateTestAthleteAsync();

        var listResponse = await CoachClient.GetAsync("/api/v1/athletes?page=1&pageSize=5");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var searchResponse = await CoachClient.GetAsync("/api/v1/athletes/search?searchTerm=test");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getByIdResponse = await CoachClient.GetAsync($"/api/v1/athletes/{athlete!.Id}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var sportsResponse = await CoachClient.GetAsync($"/api/v1/athletes/{athlete.Id}/sports");
        sportsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var achievementsResponse = await CoachClient.GetAsync($"/api/v1/athletes/{athlete.Id}/achievements");
        achievementsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

