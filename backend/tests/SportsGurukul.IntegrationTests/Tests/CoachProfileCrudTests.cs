using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class CoachProfileCrudTests : CoachIntegrationTestBase
{
    public CoachProfileCrudTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateCoach_Admin_CreatesCoachSuccessfully()
    {
        var user = Builders.TestDataBuilder.CreateUser("New Coach", "newcoach@test.com");
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var request = new CreateCoachRequest
        {
            UserId = user.Id,
            Biography = "Senior cricket coach",
            YearsOfExperience = 10,
            CurrentOrganization = "Mumbai Cricket Academy",
            HighestQualification = "BCCI Level A",
            PreferredLanguage = "English",
            CoachingLevel = CoachingLevel.Senior
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.UserId.Should().Be(user.Id);
        content.Data.CoachCode.Should().StartWith("COACH-");
        content.Data.Biography.Should().Be("Senior cricket coach");
        content.Data.YearsOfExperience.Should().Be(10);
        content.Data.CurrentOrganization.Should().Be("Mumbai Cricket Academy");
        content.Data.HighestQualification.Should().Be("BCCI Level A");
        content.Data.PreferredLanguage.Should().Be("English");
        content.Data.CoachingLevel.Should().Be("Senior");
        content.Data.Status.Should().Be("Active");
    }

    [Fact]
    public async Task CreateCoach_CoachRole_CreatesCoachSuccessfully()
    {
        var request = new CreateCoachRequest
        {
            UserId = SeedData.CoachUserId,
            Biography = "Coach self-registration",
            YearsOfExperience = 5,
            CoachingLevel = CoachingLevel.Junior
        };

        var response = await CoachClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.UserId.Should().Be(SeedData.CoachUserId);
    }

    [Fact]
    public async Task CreateCoach_AlreadyExists_ReturnsError()
    {
        await CreateTestCoachAsync(SeedData.CoachUserId);

        var request = new CreateCoachRequest
        {
            UserId = SeedData.CoachUserId,
            Biography = "Duplicate coach",
            YearsOfExperience = 3
        };

        var response = await CoachClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().NotBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateCoach_AthleteRole_ReturnsForbidden()
    {
        var user = Builders.TestDataBuilder.CreateUser("Athlete Coach", "athletecoach@test.com");
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var request = new CreateCoachRequest
        {
            UserId = user.Id,
            Biography = "Should fail",
            YearsOfExperience = 1
        };

        var response = await AthleteClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateCoach_Unauthenticated_ReturnsUnauthorized()
    {
        var request = new CreateCoachRequest
        {
            UserId = Guid.NewGuid(),
            Biography = "Should fail",
            YearsOfExperience = 1
        };

        var response = await UnauthenticatedClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCoachProfile_ExistingCoach_ReturnsProfile()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coach!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachProfileDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Coach.Should().NotBeNull();
        content.Data.Coach.Id.Should().Be(coach.Id);
        content.Data.Coach.Biography.Should().Be("Test coach biography");
    }

    [Fact]
    public async Task GetCoachProfile_NonExistent_ReturnsNotFound()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCoachById_ExistingCoach_ReturnsCoach()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coach!.Id}/details");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.Id.Should().Be(coach.Id);
    }

    [Fact]
    public async Task GetCoachByUserId_ExistingCoach_ReturnsCoach()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/user/{SeedData.CoachUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.UserId.Should().Be(SeedData.CoachUserId);
    }

    [Fact]
    public async Task GetCoachByUserId_NonExistent_ReturnsNotFound()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/user/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCoachProfile_Admin_UpdatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new UpdateCoachProfileRequest
        {
            Biography = "Updated biography",
            YearsOfExperience = 15,
            CurrentOrganization = "New Academy",
            CoachingLevel = CoachingLevel.Elite
        };

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.Biography.Should().Be("Updated biography");
        content.Data.YearsOfExperience.Should().Be(15);
        content.Data.CurrentOrganization.Should().Be("New Academy");
        content.Data.CoachingLevel.Should().Be("Elite");
    }

    [Fact]
    public async Task UpdateCoachProfile_CoachOwner_UpdatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();

        var request = new UpdateCoachProfileRequest
        {
            Biography = "Self-updated biography"
        };

        var response = await CoachClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content!.Data!.Biography.Should().Be("Self-updated biography");
    }

    [Fact]
    public async Task UpdateCoachProfile_NonExistent_ReturnsNotFound()
    {
        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach/{Guid.NewGuid()}", new UpdateCoachProfileRequest
        {
            Biography = "No-op"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCoachProfile_Unauthenticated_ReturnsUnauthorized()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}", new UpdateCoachProfileRequest
        {
            Biography = "Should fail"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteCoach_Admin_DeletesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteCoach_CoachOwner_DeletesSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();

        var response = await CoachClient.DeleteAsync($"/api/v1/coach/{coach!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteCoach_NonExistent_ReturnsNotFound()
    {
        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RestoreCoach_Admin_DeletedCoach_ReturnsNotFound()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.DeleteAsync($"/api/v1/coach/{coach!.Id}");

        var response = await AdminClient.PostAsync($"/api/v1/coach/{coach.Id}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RestoreCoach_CoachRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();
        await CoachClient.DeleteAsync($"/api/v1/coach/{coach!.Id}");

        var response = await CoachClient.PostAsync($"/api/v1/coach/{coach.Id}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ActivateCoach_Admin_ActivatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AdminClient.PostAsync($"/api/v1/coach/{coach!.Id}/activate", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content!.Data!.Status.Should().Be("Active");
    }

    [Fact]
    public async Task DeactivateCoach_Admin_DeactivatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsync($"/api/v1/coach/{coach!.Id}/activate", null);

        var response = await AdminClient.PostAsync($"/api/v1/coach/{coach!.Id}/deactivate", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
        content!.Data!.Status.Should().Be("Inactive");
    }

    [Fact]
    public async Task ActivateCoach_CoachRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();

        var response = await CoachClient.PostAsync($"/api/v1/coach/{coach!.Id}/activate", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPagedCoaches_ReturnsPagedResults()
    {
        await CreateTestCoachAsync();
        await CreateTestCoachAsync();

        var response = await UnauthenticatedClient.GetAsync("/api/v1/coach?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SportsGurukul.Application.Features.CoachManagement.DTOs.CoachSearchResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }
}
