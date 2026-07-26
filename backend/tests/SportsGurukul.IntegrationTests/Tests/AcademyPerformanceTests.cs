using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyPerformanceTests : AcademyIntegrationTestBase
{
    public AcademyPerformanceTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateAcademy_CompletesWithinTimeLimit()
    {
        var sw = Stopwatch.StartNew();
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Perf Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"perf{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "API response should be under 5 seconds");
    }

    [Fact]
    public async Task GetAcademy_CompletesWithinTimeLimit()
    {
        var academyId = await CreateAcademyAsync();

        var sw = Stopwatch.StartNew();
        var response = await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}");
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(2000,
            because: "GetAcademy should respond under 2 seconds");
    }

    [Fact]
    public async Task UpdateAcademy_CompletesWithinTimeLimit()
    {
        var academyId = await CreateAcademyAsync();

        var sw = Stopwatch.StartNew();
        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/academies/{academyId}", new
        {
            Name = "Updated Academy"
        });
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "UpdateAcademy should respond under 3 seconds");
    }

    [Fact]
    public async Task DeleteAcademy_CompletesWithinTimeLimit()
    {
        var academyId = await CreateAcademyAsync();

        var sw = Stopwatch.StartNew();
        var response = await AdminClient.DeleteAsync($"/api/v1/academies/{academyId}");
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "DeleteAcademy should respond under 3 seconds");
    }

    [Fact]
    public async Task VerifyAcademy_CompletesWithinTimeLimit()
    {
        var academyId = await CreateAcademyAsync();

        var sw = Stopwatch.StartNew();
        var response = await AcademyAdminClient.PostAsJsonAsync(
            $"/api/v1/academies/{academyId}/verify",
            new { Remarks = "Performance test verification" });
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "VerifyAcademy should respond under 3 seconds");
    }

    [Fact]
    public async Task CreateMultipleAcademies_ConcurrentRequests_CompleteWithinTimeLimit()
    {
        var tasks = Enumerable.Range(0, 5).Select(i =>
            AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
            {
                Name = $"Concurrent Academy {i} {Guid.NewGuid().ToString()[..6]}",
                Email = $"concurrent{i}{Guid.NewGuid().ToString()[..6]}@test.com",
                Phone = "+919876543210"
            }));

        var sw = Stopwatch.StartNew();
        var responses = await Task.WhenAll(tasks);
        sw.Stop();

        responses.Should().AllSatisfy(r =>
            r.StatusCode.Should().Be(HttpStatusCode.Created));
        sw.ElapsedMilliseconds.Should().BeLessThan(15000,
            because: "5 concurrent creates should complete under 15 seconds");
    }

    [Fact]
    public async Task SequentialOperations_CompleteWithinTimeLimit()
    {
        var sw = Stopwatch.StartNew();

        // Create academy
        var createResponse = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Seq Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"seq{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var content = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        var academyId = content!.Data!.Id;

        // Get academy
        await AcademyAdminClient.GetAsync($"/api/v1/academies/{academyId}");

        // Update academy
        await AcademyAdminClient.PutAsJsonAsync($"/api/v1/academies/{academyId}", new
        {
            Name = "Updated Seq Academy"
        });

        // Create branch
        await AcademyAdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/branches", new
        {
            BranchName = "Seq Branch"
        });

        // Verify academy
        await AdminClient.PostAsJsonAsync($"/api/v1/academies/{academyId}/verify",
            new { Remarks = "Seq verified" });

        // Delete academy
        await AdminClient.DeleteAsync($"/api/v1/academies/{academyId}");

        sw.Stop();
        sw.ElapsedMilliseconds.Should().BeLessThan(30000,
            because: "Full CRUD sequence should complete under 30 seconds");
    }

    private async Task<Guid> CreateAcademyAsync()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Perf Helper Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"perfhelper{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        return content?.Data?.Id ?? Guid.Empty;
    }
}
