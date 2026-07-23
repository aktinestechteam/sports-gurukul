using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AthleteCrudTests : AthleteIntegrationTestBase
{
    public AthleteCrudTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateAthlete_Admin_CreatesAthleteSuccessfully()
    {
        var user = Builders.TestDataBuilder.CreateUser("New Athlete", "newathlete@test.com");
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var request = new
        {
            UserId = user.Id,
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1,
            Height = "5'8\"",
            Weight = "65kg",
            BloodGroup = BloodGroup.BPositive,
            DominantHand = DominantHand.Left,
            DominantFoot = DominantFoot.Left,
            Biography = "New athlete bio"
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.UserId.Should().Be(user.Id);
        content.Data.AthleteCode.Should().StartWith("ATH-");
        content.Data.CurrentLevel.Should().Be("Beginner");
        content.Data.ExperienceYears.Should().Be(1);
        content.Data.Height.Should().Be("5'8\"");
        content.Data.Weight.Should().Be("65kg");
        content.Data.BloodGroup.Should().Be("BPositive");
        content.Data.DominantHand.Should().Be("Left");
        content.Data.DominantFoot.Should().Be("Left");
        content.Data.Biography.Should().Be("New athlete bio");
        content.Data.Status.Should().Be("Active");
    }

    [Fact]
    public async Task CreateAthlete_AlreadyExists_ReturnsConflict()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = SeedData.AthleteUserId,
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateAthlete_NonAdminUser_ReturnsForbidden()
    {
        var user = Builders.TestDataBuilder.CreateUser("Coach Athlete", "coachath@test.com");
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var response = await CoachClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = user.Id,
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAthlete_Unauthenticated_ReturnsUnauthorized()
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
    public async Task GetAthleteById_Exists_ReturnsAthlete()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Id.Should().Be(athlete.Id);
        content.Data.AthleteCode.Should().Be(athlete.AthleteCode);
    }

    [Fact]
    public async Task GetAthleteById_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAthleteByUserId_Exists_ReturnsAthlete()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/user/{SeedData.AthleteUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.UserId.Should().Be(SeedData.AthleteUserId);
    }

    [Fact]
    public async Task GetAthleteByUserId_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.GetAsync($"/api/v1/athletes/user/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAthletes_Admin_ReturnsPaginatedList()
    {
        await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync("/api/v1/athletes?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteSearchResponse>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAthletes_Coach_ReturnsOk()
    {
        await CreateTestAthleteAsync();

        var response = await CoachClient.GetAsync("/api/v1/athletes?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAthletes_Athlete_ReturnsForbidden()
    {
        var response = await AthleteClient.GetAsync("/api/v1/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateAthlete_Exists_UpdatesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}", new
        {
            CurrentLevel = AthleteLevel.Advanced,
            ExperienceYears = 10,
            Height = "6'0\"",
            Weight = "80kg",
            Biography = "Updated biography"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.CurrentLevel.Should().Be("Advanced");
        content.Data.ExperienceYears.Should().Be(10);
        content.Data.Height.Should().Be("6'0\"");
        content.Data.Weight.Should().Be("80kg");
        content.Data.Biography.Should().Be("Updated biography");
    }

    [Fact]
    public async Task UpdateAthlete_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{Guid.NewGuid()}", new
        {
            CurrentLevel = AthleteLevel.Advanced
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAthlete_PartialUpdate_OnlyUpdatesSuppliedFields()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}", new
        {
            Biography = "Only bio updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Biography.Should().Be("Only bio updated");
        content.Data.Height.Should().Be("5'10\"");
    }

    [Fact]
    public async Task DeleteAthlete_Admin_DeletesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbAthlete = await GetAthleteFromDbAsync(athlete.Id);
        dbAthlete.Should().NotBeNull();
        dbAthlete!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAthlete_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAthlete_NonAdmin_ReturnsForbidden()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await CoachClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RestoreAthlete_Admin_RestoresSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}");

        var response = await AdminClient.PostAsync($"/api/v1/athletes/{athlete.Id}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        content!.Success.Should().BeTrue();

        var dbAthlete = await GetAthleteFromDbAsync(athlete.Id);
        dbAthlete.Should().NotBeNull();
        dbAthlete!.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task RestoreAthlete_NotDeleted_ReturnsBadRequest()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RestoreAthlete_NonAdmin_ReturnsForbidden()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}");

        var response = await CoachClient.PostAsync($"/api/v1/athletes/{athlete.Id}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

