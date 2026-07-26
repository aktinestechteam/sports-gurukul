using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyAuthorizationTests : AcademyIntegrationTestBase
{
    public AcademyAuthorizationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    #region Academy CRUD Authorization

    [Fact]
    public async Task CreateAcademy_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Test",
            Email = "test@test.com",
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAcademy_AthleteRole_Returns403()
    {
        var response = await AthleteClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Test",
            Email = "test@test.com",
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAcademy_CoachRole_Returns403()
    {
        var response = await CoachClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Test",
            Email = "test@test.com",
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAcademy_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/academies/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateAcademy_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PutAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}", new
        {
            Name = "Updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateAcademy_AthleteRole_Returns403()
    {
        var response = await AthleteClient.PutAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}", new
        {
            Name = "Updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAcademy_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.DeleteAsync($"/api/v1/academies/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteAcademy_AthleteRole_Returns403()
    {
        var response = await AthleteClient.DeleteAsync($"/api/v1/academies/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAcademy_AcademyAdminRole_Returns403()
    {
        var response = await AcademyAdminClient.DeleteAsync($"/api/v1/academies/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RestoreAcademy_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsync($"/api/v1/academies/{Guid.NewGuid()}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RestoreAcademy_AthleteRole_Returns403()
    {
        var response = await AthleteClient.PostAsync($"/api/v1/academies/{Guid.NewGuid()}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task VerifyAcademy_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/verify", new { Remarks = "test" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task VerifyAcademy_AthleteRole_Returns403()
    {
        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/verify", new { Remarks = "test" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task VerifyAcademy_AcademyAdminRole_Returns403()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/verify", new { Remarks = "test" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectAcademy_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/reject", new { Remarks = "test" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RejectAcademy_AthleteRole_Returns403()
    {
        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/reject", new { Remarks = "test" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Branch Authorization

    [Fact]
    public async Task CreateBranch_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/branches", new
        {
            BranchName = "Test"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBranch_AthleteRole_Returns403()
    {
        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/branches", new
        {
            BranchName = "Test"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetBranches_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/academies/{Guid.NewGuid()}/branches");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Facility Authorization

    [Fact]
    public async Task CreateFacility_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = Guid.NewGuid(),
            FacilityName = "Test",
            FacilityType = Domain.Enums.FacilityType.BadmintonCourt,
            Capacity = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFacilities_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/facilities");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Membership Authorization

    [Fact]
    public async Task CreateMembership_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}/memberships", new
        {
            MembershipName = "Test",
            Price = 1000.00m,
            Duration = 30
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMemberships_Unauthenticated_Returns401()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/academies/{Guid.NewGuid()}/memberships");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Admin Role Can Access All

    [Fact]
    public async Task SystemAdmin_CanAccessAllAcademyEndpoints()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var endpoints = new Func<Task<HttpResponseMessage>>[]
        {
            () => AdminClient.GetAsync($"/api/v1/academies/{academyId}"),
            () => AdminClient.PutAsJsonAsync($"/api/v1/academies/{academyId}", new { Name = "Updated" }),
            () => AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/verify", new { Remarks = "Verified" }),
            () => AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/reject", new { Remarks = "Rejected" }),
            () => AdminClient.PostAsync($"/api/v1/academies/{academyId}/restore", null),
        };

        foreach (var endpoint in endpoints)
        {
            var response = await endpoint();
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden,
                because: $"System Admin should not get 403 on {response.RequestMessage!.RequestUri}");
        }
    }

    #endregion

    private async Task<Guid> CreateAcademyViaApiAsync()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Auth Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"authtest{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        return content?.Data?.Id ?? Guid.Empty;
    }
}
