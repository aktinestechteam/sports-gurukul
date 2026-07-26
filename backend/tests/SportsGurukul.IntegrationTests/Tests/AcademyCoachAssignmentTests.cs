using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyCoachAssignmentTests : AcademyIntegrationTestBase
{
    public AcademyCoachAssignmentTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task AssignCoach_ValidCoach_Returns201()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var coach = await CreateCoachDirectlyInDbAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyCoachSummaryDto>>();
        content!.Data.Should().NotBeNull();
        content.Data!.CoachId.Should().Be(coach.Id);
    }

    [Fact]
    public async Task AssignCoach_NonExistentAcademy_Returns404()
    {
        var coach = await CreateCoachDirectlyInDbAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{Guid.NewGuid()}/coaches/{coach.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignCoach_NonExistentCoach_Returns404()
    {
        var academyId = await CreateVerifiedAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{Guid.NewGuid()}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignCoach_DuplicateAssignment_Returns409()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var coach = await CreateCoachDirectlyInDbAsync();

        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}", new { });

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetAssignedCoaches_Returns200()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var coach = await CreateCoachDirectlyInDbAsync();
        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}", new { });

        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}/coaches");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AcademyCoachSummaryDto>>>();
        content!.Data.Should().Contain(c => c.CoachId == coach.Id);
    }

    [Fact]
    public async Task RemoveCoach_AssignedCoach_Returns204()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var coach = await CreateCoachDirectlyInDbAsync();
        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}", new { });

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveCoach_NonExistentAssignment_Returns404()
    {
        var academyId = await CreateVerifiedAcademyAsync();

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/coaches/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignCoach_AthleteRole_Returns403()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var coach = await CreateCoachDirectlyInDbAsync();

        var response = await AthleteClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AssignCoach_CoachRole_Returns403()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var coach = await CreateCoachDirectlyInDbAsync();

        var response = await CoachClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AssignCoach_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync(
            $"/api/v1/academies/{Guid.NewGuid()}/coaches/{Guid.NewGuid()}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAssignedCoaches_AthleteCanAccess()
    {
        var academyId = await CreateVerifiedAcademyAsync();

        var response = await AthleteClient.GetAsync($"/api/v1/academies/{academyId}/coaches");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveCoach_AthleteRole_Returns403()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var coach = await CreateCoachDirectlyInDbAsync();

        var response = await AthleteClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/coaches/{coach.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateVerifiedAcademyAsync()
    {
        var createResponse = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Coach Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"coach{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        var academyId = createContent!.Data!.Id;

        await AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/verify",
            new { Remarks = "Verified" });

        return academyId;
    }
}
