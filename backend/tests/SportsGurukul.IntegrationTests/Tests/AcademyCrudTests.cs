using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyCrudTests : AcademyIntegrationTestBase
{
    public AcademyCrudTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateAcademy_Admin_CreatesSuccessfully()
    {
        var request = new
        {
            Name = "Mumbai Sports Academy",
            LegalName = "Mumbai Sports Academy Pvt. Ltd.",
            Description = "Premier multi-sport academy",
            Email = $"createacademy{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210",
            EstablishedDate = new DateTime(2020, 6, 15),
            Website = "https://mumbaisportsacademy.com"
        };

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Name.Should().Be("Mumbai Sports Academy");
        content.Data.AcademyCode.Should().StartWith("ACAD-");
        content.Data.Email.Should().Be(request.Email);
        content.Data.Phone.Should().Be("+919876543210");
        content.Data.Status.Should().Be("Pending");
        content.Data.VerificationStatus.Should().Be("Pending");
    }

    [Fact]
    public async Task CreateAcademy_SystemAdmin_CreatesSuccessfully()
    {
        var request = new
        {
            Name = "Delhi Sports Academy",
            Email = $"delhi{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543211"
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/academies", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateAcademy_DuplicateEmail_ReturnsConflict()
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
    public async Task GetAcademyById_Exists_ReturnsAcademy()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Id.Should().Be(academyId);
    }

    [Fact]
    public async Task GetAcademyById_NotExists_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAcademy_Exists_UpdatesSuccessfully()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/academies/{academyId}", new
        {
            Name = "Updated Academy Name",
            Description = "Updated description"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Name.Should().Be("Updated Academy Name");
        content.Data.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateAcademy_NotExists_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/academies/{Guid.NewGuid()}", new
        {
            Name = "Updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAcademy_Admin_DeletesSuccessfully()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AdminClient.DeleteAsync($"/api/v1/academies/{academyId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var dbAcademy = await GetAcademyFromDbAsync(academyId);
        dbAcademy.Should().NotBeNull();
        dbAcademy!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAcademy_AcademyAdmin_ReturnsForbidden()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AcademyAdminClient.DeleteAsync($"/api/v1/academies/{academyId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAcademy_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.DeleteAsync($"/api/v1/academies/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RestoreAcademy_Admin_RestoresSuccessfully()
    {
        var academyId = await CreateAcademyViaApiAsync();
        await AdminClient.DeleteAsync($"/api/v1/academies/{academyId}");

        var response = await AdminClient.PostAsync($"/api/v1/academies/{academyId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dbAcademy = await GetAcademyFromDbAsync(academyId);
        dbAcademy.Should().NotBeNull();
        dbAcademy!.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task RestoreAcademy_NotDeleted_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AdminClient.PostAsync($"/api/v1/academies/{academyId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyAcademy_Admin_VerifiesSuccessfully()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/verify", new
        {
            Remarks = "Documentation verified"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.VerificationStatus.Should().Be("Verified");
    }

    [Fact]
    public async Task VerifyAcademy_AcademyAdmin_ReturnsForbidden()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/verify", new
        {
            Remarks = "Verified"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectAcademy_Admin_RejectsSuccessfully()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/reject", new
        {
            Remarks = "Registration document expired"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.VerificationStatus.Should().Be("Rejected");
    }

    [Fact]
    public async Task RejectAcademy_EmptyRemarks_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyViaApiAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/reject", new
        {
            Remarks = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAcademy_Performance_CompletesWithinTimeLimit()
    {
        var sw = Stopwatch.StartNew();
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = "Perf Academy",
            Email = $"perf{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "creating an academy should complete within 5 seconds");
    }
}
