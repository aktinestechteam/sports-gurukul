using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class CoachAuthIntegrationTests : CoachIntegrationTestBase
{
    public CoachAuthIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

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
    public async Task DeleteCoach_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.DeleteAsync($"/api/v1/coach/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ActivateCoach_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.PostAsync($"/api/v1/coach/{Guid.NewGuid()}/activate", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCoach_AthleteRole_ReturnsForbidden()
    {
        var user = Builders.TestDataBuilder.CreateUser("Athlete Coach", "athletecoachauth@test.com");
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
    public async Task UpdateCoachProfile_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}", new UpdateCoachProfileRequest
        {
            Biography = "Should fail"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCoach_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.DeleteAsync($"/api/v1/coach/{coach!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ActivateCoach_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsync($"/api/v1/coach/{coach!.Id}/activate", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeactivateCoach_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsync($"/api/v1/coach/{coach!.Id}/deactivate", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RestoreCoach_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsync($"/api/v1/coach/{coach!.Id}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AssignSport_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", new CoachAssignSportRequest
        {
            SportId = SeedData.CricketSportId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RemoveSport_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.DeleteAsync($"/api/v1/coach/{coach!.Id}/sports/{SeedData.CricketSportId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddCertification_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", new AddCertificationRequest
        {
            CertificationName = "Should Fail",
            IssuingAuthority = "Org",
            IssueDate = DateTime.UtcNow
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AssignAthlete_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync<object?>(
            $"/api/v1/coach/{coach!.Id}/athletes/{Guid.NewGuid()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddExperience_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/experience", new AddExperienceRequest
        {
            Organization = "Org",
            Role = "Should Fail",
            StartDate = DateTime.UtcNow
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddEducation_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/education", new AddEducationRequest
        {
            Degree = "Should Fail",
            Institution = "Uni"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCoachProfile_Anonymous_ReturnsOk()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coach!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCoachById_Anonymous_ReturnsOk()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coach!.Id}/details");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCoachByUserId_Anonymous_ReturnsNotFound()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/user/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSports_Anonymous_ReturnsOk()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coach!.Id}/sports");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCertifications_Anonymous_ReturnsOk()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coach!.Id}/certifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
