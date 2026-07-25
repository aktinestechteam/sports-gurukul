using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class CoachAthleteAssignmentTests : CoachIntegrationTestBase
{
    public CoachAthleteAssignmentTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    private async Task VerifyCoachAsync(Guid coachId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var coach = await dbContext.Coaches.FirstAsync(c => c.Id == coachId);
        coach.VerificationStatus = VerificationStatus.Verified;
        coach.Status = CoachStatus.Active;
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task AssignAthlete_Admin_AssignsSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await VerifyCoachAsync(coach!.Id);

        var response = await AdminClient.PostAsJsonAsync<object?>(
            $"/api/v1/coach/{coach.Id}/athletes/{SeedData.AthleteId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AssignedAthleteDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task AssignAthlete_CoachOwner_AssignsSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();
        await VerifyCoachAsync(coach!.Id);

        var response = await CoachClient.PostAsJsonAsync<object?>(
            $"/api/v1/coach/{coach.Id}/athletes/{SeedData.AthleteId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignAthlete_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync<object?>(
            $"/api/v1/coach/{coach!.Id}/athletes/{SeedData.AthleteId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAssignedAthletes_WithAssignments_ReturnsList()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await VerifyCoachAsync(coach!.Id);
        await AdminClient.PostAsJsonAsync<object?>(
            $"/api/v1/coach/{coach.Id}/athletes/{SeedData.AthleteId}", null);

        var response = await AdminClient.GetAsync($"/api/v1/coach/{coach.Id}/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AssignedAthleteDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetAssignedAthletes_NoAssignments_ReturnsEmpty()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AdminClient.GetAsync($"/api/v1/coach/{coach!.Id}/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AssignedAthleteDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveAthlete_Admin_RemovesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await VerifyCoachAsync(coach!.Id);
        await AdminClient.PostAsJsonAsync<object?>(
            $"/api/v1/coach/{coach.Id}/athletes/{SeedData.AthleteId}", null);

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach.Id}/athletes/{SeedData.AthleteId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveAthlete_CoachOwner_RemovesSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();
        await VerifyCoachAsync(coach!.Id);
        await CoachClient.PostAsJsonAsync<object?>(
            $"/api/v1/coach/{coach.Id}/athletes/{SeedData.AthleteId}", null);

        var response = await CoachClient.DeleteAsync($"/api/v1/coach/{coach.Id}/athletes/{SeedData.AthleteId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveAthlete_NonExistent_ReturnsOk()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach!.Id}/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignAthlete_AthleteRole_ReturnsForbidden_GetAthletes()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AthleteClient.GetAsync($"/api/v1/coach/{coach!.Id}/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
