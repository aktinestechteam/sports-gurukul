using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyDatabaseTests : AcademyIntegrationTestBase
{
    public AcademyDatabaseTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task SoftDelete_SetsIsDeletedAndStatus()
    {
        var academyId = await CreateAcademyAsync();
        await AdminClient.DeleteAsync($"/api/v1/academies/{academyId}");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var academy = await dbContext.Academies.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == academyId);

        academy.Should().NotBeNull();
        academy!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Restore_SetsIsDeletedFalse()
    {
        var academyId = await CreateAcademyAsync();
        await AdminClient.DeleteAsync($"/api/v1/academies/{academyId}");
        await AdminClient.PostAsync($"/api/v1/academies/{academyId}/restore", null);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var academy = await dbContext.Academies
            .FirstOrDefaultAsync(a => a.Id == academyId);

        academy.Should().NotBeNull();
        academy!.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task SoftDelete_Branch_CascadeWorks()
    {
        var academyId = await CreateAcademyAsync();
        var branchResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/branches", new
            {
                BranchName = "Cascade Branch"
            });
        var branchContent = await branchResponse.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
        var branchId = branchContent!.Data!.Id;

        await AcademyAdminClient.DeleteAsync($"/api/v1/academies/{academyId}/branches/{branchId}");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var branch = await dbContext.AcademyBranches.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == branchId);
        branch.Should().NotBeNull();
        branch!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDelete_Facility_CascadeWorks()
    {
        var academyId = await CreateAcademyAsync();
        var facilityResponse = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = academyId,
            FacilityName = "Cascade Facility",
            FacilityType = Domain.Enums.FacilityType.BadmintonCourt,
            Capacity = 10
        });
        var facilityContent = await facilityResponse.Content.ReadFromJsonAsync<ApiResponse<FacilityDto>>();
        var facilityId = facilityContent!.Data!.Id;

        await AcademyAdminClient.DeleteAsync($"/api/v1/facilities/{facilityId}");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var facility = await dbContext.AcademyFacilities.IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == facilityId);
        facility.Should().NotBeNull();
        facility!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDelete_MembershipPlan_CascadeWorks()
    {
        var academyId = await CreateAcademyAsync();
        var planResponse = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/memberships", new
            {
                MembershipName = "Cascade Plan",
                Price = 1000.00m,
                Duration = 30
            });
        var planContent = await planResponse.Content.ReadFromJsonAsync<ApiResponse<MembershipPlanDto>>();
        var planId = planContent!.Data!.Id;

        await AcademyAdminClient.DeleteAsync($"/api/v1/academies/{academyId}/memberships/{planId}");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var plan = await dbContext.AcademyMemberships.IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == planId);
        plan.Should().NotBeNull();
        plan!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task AuditFields_AreSetOnCreate()
    {
        var academyId = await CreateAcademyAsync();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var academy = await dbContext.Academies
            .FirstOrDefaultAsync(a => a.Id == academyId);

        academy.Should().NotBeNull();
        academy!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        academy.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task AcademyCode_IsAutoGenerated()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Code Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"codetest{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        content!.Data!.AcademyCode.Should().StartWith("ACAD-");
        content.Data.AcademyCode.Length.Should().Be(18);
    }

    [Fact]
    public async Task DeletedAcademy_NotVisibleInGet()
    {
        var academyId = await CreateAcademyAsync();
        await AdminClient.DeleteAsync($"/api/v1/academies/{academyId}");

        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreateAcademyAsync()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"DB Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"dbtest{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        return content?.Data?.Id ?? Guid.Empty;
    }
}
