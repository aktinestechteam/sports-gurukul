using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyBranchTests : AcademyIntegrationTestBase
{
    public AcademyBranchTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    private async Task<Guid> CreateAcademyAndGetIdAsync()
    {
        var request = new
        {
            Name = $"Branch Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"branchacademy{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        };
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", request);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        return content!.Data!.Id;
    }

    [Fact]
    public async Task CreateBranch_ValidRequest_ReturnsCreated()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Andheri Branch",
                Address = "123 Sports Avenue",
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India",
                PostalCode = "400058",
                Latitude = 19.1364m,
                Longitude = 72.8296m
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.BranchName.Should().Be("Andheri Branch");
        content.Data.City.Should().Be("Mumbai");
    }

    [Fact]
    public async Task CreateBranch_AcademyNotFound_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{Guid.NewGuid()}/branches", new
            {
                BranchName = "Test Branch"
            });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateBranch_EmptyName_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = ""
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBranches_Exists_ReturnsList()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Branch A"
            });

        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}/branches");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BranchDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().HaveCount(1);
        content.Data![0].BranchName.Should().Be("Branch A");
    }

    [Fact]
    public async Task GetBranchById_Exists_ReturnsBranch()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Branch B"
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        var branchId = created!.Data!.Id;

        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}/branches/{branchId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBranchById_NotExists_ReturnsNotFound()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}/branches/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBranch_Exists_UpdatesSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Old Branch"
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        var branchId = created!.Data!.Id;

        var response = await AcademyAdminClient.PutAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches/{branchId}", new
            {
                BranchName = "New Branch",
                City = "Mumbai"
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        content!.Data!.BranchName.Should().Be("New Branch");
        content.Data.City.Should().Be("Mumbai");
    }

    [Fact]
    public async Task DeleteBranch_Exists_DeletesSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "To Delete"
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        var branchId = created!.Data!.Id;

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/branches/{branchId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBranch_NotExists_ReturnsNotFound()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.DeleteAsync(
            $"/api/v1/academies/{academyId}/branches/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RestoreBranch_Exists_RestoresSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "To Restore"
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        var branchId = created!.Data!.Id;
        await AcademyAdminClient.DeleteAsync($"/api/v1/academies/{academyId}/branches/{branchId}");

        var response = await AcademyAdminClient.PostAsync(
            $"/api/v1/academies/{academyId}/branches/{branchId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RestoreBranch_NotDeleted_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var createResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Not Deleted Branch"
            });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        var branchId = created!.Data!.Id;

        var response = await AcademyAdminClient.PostAsync(
            $"/api/v1/academies/{academyId}/branches/{branchId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBranch_CoachRole_ReturnsForbidden()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await CoachClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Coach Branch"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBranch_AthleteRole_ReturnsForbidden()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AthleteClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Athlete Branch"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBranch_Unauthenticated_ReturnsUnauthorized()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await UnauthenticatedClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Unauth Branch"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
