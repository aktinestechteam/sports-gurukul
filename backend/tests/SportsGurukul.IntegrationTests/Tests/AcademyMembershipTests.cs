using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyMembershipTests : AcademyIntegrationTestBase
{
    public AcademyMembershipTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    private async Task<Guid> CreateAcademyAndGetIdAsync()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Membership Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"membership{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        return content!.Data!.Id;
    }

    [Fact]
    public async Task CreateMembershipPlan_ValidRequest_ReturnsCreated()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Gold Monthly Plan",
                Description = "Unlimited access for 30 days",
                Price = 2500.00m,
                Duration = 30,
                Benefits = "Unlimited gym access, 2 coaching sessions, locker access"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.MembershipName.Should().Be("Gold Monthly Plan");
        content.Data.Price.Should().Be(2500.00m);
        content.Data.Duration.Should().Be(30);
        content.Data.Status.Should().Be("Active");
    }

    [Fact]
    public async Task CreateMembershipPlan_EmptyName_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "",
                Price = 1000.00m,
                Duration = 30
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateMembershipPlan_AcademyNotFound_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{Guid.NewGuid()}/memberships", new
            {
                MembershipName = "Test Plan",
                Price = 1000.00m,
                Duration = 30
            });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMembershipPlans_Exists_ReturnsList()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Silver Plan",
                Price = 1500.00m,
                Duration = 30
            });

        var response = await AcademyAdminClient.GetAsync(
            $"/api/v1/academies/{academyId}/memberships");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<MembershipPlanDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().HaveCount(1);
        content.Data![0].MembershipName.Should().Be("Silver Plan");
    }

    [Fact]
    public async Task GetMembershipPlanById_Exists_ReturnsPlan()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Gold Plan",
                Price = 2500.00m,
                Duration = 30
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        var planId = created!.Data!.Id;

        var response = await AcademyAdminClient.GetAsync(
            $"/api/v1/academies/{academyId}/memberships/{planId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        content!.Data!.MembershipName.Should().Be("Gold Plan");
    }

    [Fact]
    public async Task GetMembershipPlanById_NotExists_ReturnsNotFound()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.GetAsync(
            $"/api/v1/academies/{academyId}/memberships/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateMembershipPlan_Exists_UpdatesSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Old Plan",
                Price = 1000.00m,
                Duration = 30
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        var planId = created!.Data!.Id;

        var response = await AcademyAdminClient.PutAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships/{planId}", new
            {
                MembershipName = "Updated Plan",
                Price = 2000.00m,
                Duration = 60
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        content!.Data!.MembershipName.Should().Be("Updated Plan");
        content.Data.Price.Should().Be(2000.00m);
    }

    [Fact]
    public async Task DeleteMembershipPlan_Exists_DeletesSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "To Delete",
                Price = 1000.00m,
                Duration = 30
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        var planId = created!.Data!.Id;

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/memberships/{planId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteMembershipPlan_NotExists_ReturnsNotFound()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/memberships/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ActivateMembershipPlan_DeactivatedPlan_ReturnsOk()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "To Activate",
                Price = 1000.00m,
                Duration = 30
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        var planId = created!.Data!.Id;
        await AcademyAdminClient.PostAsync(
            $"/api/v1/academies/{academyId}/memberships/{planId}/deactivate", null);

        var response = await AcademyAdminClient.PostAsync(
            $"/api/v1/academies/{academyId}/memberships/{planId}/activate", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        content!.Data!.Status.Should().Be("Active");
    }

    [Fact]
    public async Task DeactivateMembershipPlan_ActivePlan_ReturnsOk()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "To Deactivate",
                Price = 1000.00m,
                Duration = 30
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        var planId = created!.Data!.Id;

        var response = await AcademyAdminClient.PostAsync(
            $"/api/v1/academies/{academyId}/memberships/{planId}/deactivate", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        content!.Data!.Status.Should().Be("Inactive");
    }

    [Fact]
    public async Task Membership_Authorization_AthleteRole_ForbiddenForCreate()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AthleteClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Test",
                Price = 1000.00m,
                Duration = 30
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Membership_Authorization_AthleteRole_CanGetList()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AthleteClient.GetAsync(
            $"/api/v1/academies/{academyId}/memberships");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Membership_Authorization_Unauthenticated_ReturnsUnauthorized()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await UnauthenticatedClient.GetAsync(
            $"/api/v1/academies/{academyId}/memberships");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}