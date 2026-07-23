using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class ValidationTests : AthleteIntegrationTestBase
{
    public ValidationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateAthlete_EmptyUserId_ReturnsBadRequest()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = Guid.Empty,
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAthlete_NegativeExperience_ReturnsBadRequest()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = Guid.NewGuid(),
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = -1
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAthlete_InvalidBloodGroup_ReturnsBadRequest()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = Guid.NewGuid(),
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1,
            BloodGroup = (BloodGroup)999
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAthlete_InvalidDominantHand_ReturnsBadRequest()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = Guid.NewGuid(),
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1,
            DominantHand = (DominantHand)999
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAthlete_EmptyBody_StillReturnsOk()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddAchievement_EmptyTitle_ReturnsBadRequest()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "",
            Competition = "Test",
            Level = AchievementLevel.Local,
            Date = DateTime.UtcNow
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddAchievement_InvalidLevel_ReturnsBadRequest()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "Test",
            Level = (AchievementLevel)999,
            Date = DateTime.UtcNow
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateEmergencyContact_EmptyName_ReturnsBadRequest()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact", new
        {
            Name = "",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateEmergencyContact_EmptyPhone_ReturnsBadRequest()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact", new
        {
            Name = "Test Contact",
            Relationship = EmergencyRelationship.Parent,
            Phone = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateEmergencyContact_InvalidRelationship_ReturnsBadRequest()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact", new
        {
            Name = "Test Contact",
            Relationship = (EmergencyRelationship)999,
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AssignSport_EmptySportId_ReturnsBadRequest()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = Guid.Empty,
            IsPrimarySport = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRanking_EmptyBody_StillReturnsOk()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/ranking", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateMedicalProfile_EmptyBody_StillReturnsOk()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/medical-profile", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

