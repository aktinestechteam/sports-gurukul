using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyAthleteRegistrationTests : AcademyIntegrationTestBase
{
    public AcademyAthleteRegistrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task RegisterAthlete_ValidAthlete_Returns201()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyAthleteSummaryDto>>();
        content!.Data.Should().NotBeNull();
        content.Data!.AthleteId.Should().Be(athlete.Id);
    }

    [Fact]
    public async Task RegisterAthlete_NonExistentAcademy_Returns404()
    {
        var athlete = await CreateAthleteDirectlyInDbAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{Guid.NewGuid()}/athletes/{athlete.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterAthlete_NonExistentAthlete_Returns404()
    {
        var academyId = await CreateVerifiedAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{Guid.NewGuid()}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterAthlete_DuplicateRegistration_Returns409()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();

        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetRegisteredAthletes_Returns200()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();
        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AcademyAthleteSummaryDto>>>();
        content!.Data.Should().Contain(a => a.AthleteId == athlete.Id);
    }

    [Fact]
    public async Task RemoveAthlete_RegisteredAthlete_Returns204()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();
        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveAthlete_NonExistentRegistration_Returns404()
    {
        var academyId = await CreateVerifiedAcademyAsync();

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/athletes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TransferAthlete_ValidTransfer_Returns200()
    {
        var fromAcademyId = await CreateVerifiedAcademyAsync();
        var toAcademyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();

        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{fromAcademyId}/athletes/{athlete.Id}", new { });

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{fromAcademyId}/athletes/{athlete.Id}/transfer",
            new { ToAcademyId = toAcademyId });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyAthleteSummaryDto>>();
        content!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task TransferAthlete_ToSameAcademy_Returns400Or409()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();

        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}/transfer",
            new { ToAcademyId = academyId });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task TransferAthlete_AthleteNotInSourceAcademy_Returns400Or404()
    {
        var fromAcademyId = await CreateVerifiedAcademyAsync();
        var toAcademyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{fromAcademyId}/athletes/{athlete.Id}/transfer",
            new { ToAcademyId = toAcademyId });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterAthlete_AthleteRole_Returns403()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();

        var response = await AthleteClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegisterAthlete_CoachRole_Returns403()
    {
        var academyId = await CreateVerifiedAcademyAsync();
        var athlete = await CreateAthleteDirectlyInDbAsync();

        var response = await CoachClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/athletes/{athlete.Id}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegisterAthlete_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync(
            $"/api/v1/academies/{Guid.NewGuid()}/athletes/{Guid.NewGuid()}", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRegisteredAthletes_AthleteCanAccess()
    {
        var academyId = await CreateVerifiedAcademyAsync();

        var response = await AthleteClient.GetAsync($"/api/v1/academies/{academyId}/athletes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateVerifiedAcademyAsync()
    {
        var createResponse = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Athlete Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"athlete{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        var academyId = createContent!.Data!.Id;

        await AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/verify",
            new { Remarks = "Verified" });

        return academyId;
    }
}