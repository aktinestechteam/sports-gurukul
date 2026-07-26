using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyValidationTests : AcademyIntegrationTestBase
{
    public AcademyValidationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateAcademy_EmptyName_ReturnsBadRequest()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "",
            Email = "test@test.com",
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAcademy_InvalidEmail_ReturnsBadRequest()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Test Academy",
            Email = "not-an-email",
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAcademy_EmptyEmail_ReturnsBadRequest()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Test Academy",
            Email = "",
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAcademy_EmptyPhone_ReturnsBadRequest()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBranch_EmptyName_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = ""
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFacility_EmptyName_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = academyId,
            FacilityName = "",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFacility_InvalidAcademyId_ReturnsBadRequest()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = Guid.Empty,
            FacilityName = "Test",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFacility_ZeroCapacity_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = academyId,
            FacilityName = "Test Facility",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 0
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateMembershipPlan_EmptyName_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

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
    public async Task CreateMembershipPlan_NegativePrice_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Test Plan",
                Price = -100.00m,
                Duration = 30
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateMembershipPlan_ZeroDuration_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Test Plan",
                Price = 1000.00m,
                Duration = 0
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RejectAcademy_EmptyRemarks_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/reject", new
        {
            Remarks = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAcademy_InvalidWebsite_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAsync();

        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/academies/{academyId}", new
        {
            Website = "not-a-url"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DuplicateAcademyName_ReturnsConflict()
    {
        var name = $"Unique Academy {Guid.NewGuid().ToString()[..6]}";
        await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = name,
            Email = $"unique1{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = name,
            Email = $"unique2{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543211"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DuplicateAcademyEmail_ReturnsConflict()
    {
        var email = $"dupe{Guid.NewGuid().ToString()[..6]}@test.com";
        await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "First Academy",
            Email = email,
            Phone = "+919876543210"
        });

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Second Academy",
            Email = email,
            Phone = "+919876543211"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<Guid> CreateAcademyAsync()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Validation Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"validation{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        return content?.Data?.Id ?? Guid.Empty;
    }
}
