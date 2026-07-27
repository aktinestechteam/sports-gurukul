using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class ProgressApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    private const string EnrollmentsBaseRoute = "api/v1/enrollments";
    private const string ProgramsBaseRoute = "api/v1/training-programs";
    private const string BatchesBaseRoute = "api/v1/training-batches";
    private const string CertificatesBaseRoute = "api/v1/certificates";
    private const string ProgressBaseRoute = "api/v1/progress";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ProgressApiTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region Helpers

    private async Task<Guid> CreateProgramAsync(HttpClient client)
    {
        var body = new
        {
            ProgramName = $"Test Program {Guid.NewGuid().ToString()[..6]}",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Test program for progress integration tests",
            DifficultyLevel = 0,
            MinimumAge = 8,
            MaximumAge = 18,
            DurationWeeks = 24,
            Capacity = 30
        };

        var response = await PostJsonAsync(client, ProgramsBaseRoute, body);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task<Guid> CreateBatchAsync(HttpClient client, Guid programId)
    {
        var body = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };

        var response = await PostJsonAsync(client, $"{ProgramsBaseRoute}/{programId}/batches", body);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task StartBatchAsync(HttpClient client, Guid batchId)
    {
        var response = await PostAsync(client, $"{BatchesBaseRoute}/{batchId}/start");
        response.EnsureSuccessStatusCode();
    }

    private async Task<Guid> EnrollAthleteAsync(HttpClient client, Guid batchId)
    {
        var body = new
        {
            athleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(client, $"{BatchesBaseRoute}/{batchId}/enrollments", body);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EnrollmentDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task<(Guid ProgramId, Guid BatchId, Guid EnrollmentId)> CreateEnrollmentFixtureAsync(HttpClient client)
    {
        var programId = await CreateProgramAsync(client);
        var batchId = await CreateBatchAsync(client, programId);
        await StartBatchAsync(client, batchId);
        var enrollmentId = await EnrollAthleteAsync(client, batchId);
        return (programId, batchId, enrollmentId);
    }

    private async Task SeedMilestoneAsync(Guid programId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var existingMilestone = await dbContext.TrainingMilestones
            .FirstOrDefaultAsync(m => m.ProgramId == programId && m.MilestoneName == "Week 4 Milestone");

        if (existingMilestone is not null)
            return;

        var milestone = new TrainingMilestone
        {
            ProgramId = programId,
            MilestoneName = "Week 4 Milestone",
            Description = "Complete basic drills",
            WeekNumber = 4,
            IsCompleted = false
        };

        dbContext.TrainingMilestones.Add(milestone);
        await dbContext.SaveChangesAsync();
    }

    private async Task<Guid> GetMilestoneIdAsync(Guid programId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var milestone = await dbContext.TrainingMilestones
            .FirstOrDefaultAsync(m => m.ProgramId == programId && m.MilestoneName == "Week 4 Milestone");
        return milestone?.Id ?? Guid.Empty;
    }

    #endregion

    #region Get Progress

    [Fact]
    public async Task GetProgress_ReturnsOk_WhenEnrollmentExists()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, enrollmentId) = await CreateEnrollmentFixtureAsync(adminClient);

        var response = await GetAsync(adminClient, $"{EnrollmentsBaseRoute}/{enrollmentId}/progress");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgressDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(enrollmentId, content.Data!.EnrollmentId);
    }

    [Fact]
    public async Task GetProgress_Returns404_WhenEnrollmentNotFound()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var nonExistentEnrollmentId = Guid.NewGuid();

        var response = await GetAsync(adminClient, $"{EnrollmentsBaseRoute}/{nonExistentEnrollmentId}/progress");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProgress_ReturnsOk_WithProgressData()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, enrollmentId) = await CreateEnrollmentFixtureAsync(adminClient);

        var updateBody = new
        {
            currentLevel = "Intermediate",
            completedPercentage = 45.5m,
            overallRating = 4.2m
        };
        var updateResponse = await PutJsonAsync(adminClient, $"{EnrollmentsBaseRoute}/{enrollmentId}/progress", updateBody);
        updateResponse.EnsureSuccessStatusCode();

        var response = await GetAsync(adminClient, $"{EnrollmentsBaseRoute}/{enrollmentId}/progress");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgressDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(enrollmentId, content.Data!.EnrollmentId);
        Assert.Equal("Intermediate", content.Data.CurrentLevel);
        Assert.Equal(45.5m, content.Data.CompletedPercentage);
        Assert.Equal(4.2m, content.Data.OverallRating);
    }

    #endregion

    #region Issue Certificate

    [Fact]
    public async Task IssueCertificate_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, enrollmentId) = await CreateEnrollmentFixtureAsync(adminClient);

        var body = new
        {
            certificateType = "Completion",
            fileUrl = "https://example.com/cert.pdf"
        };

        var response = await PostJsonAsync(adminClient, $"{EnrollmentsBaseRoute}/{enrollmentId}/certificate", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CertificateDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("Completion", content.Data!.CertificateType);
        Assert.Equal(enrollmentId, content.Data.EnrollmentId);
        Assert.Equal("https://example.com/cert.pdf", content.Data.FileUrl);
        Assert.False(string.IsNullOrEmpty(content.Data.CertificateNumber));
    }

    [Fact]
    public async Task IssueCertificate_Returns401_WhenAnonymous()
    {
        var anonymousClient = CreateAnonymousClient();
        var enrollmentId = Guid.NewGuid();

        var body = new
        {
            certificateType = "Completion",
            fileUrl = "https://example.com/cert.pdf"
        };

        var response = await PostJsonAsync(anonymousClient, $"{EnrollmentsBaseRoute}/{enrollmentId}/certificate", body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task IssueCertificate_Returns403_WhenAthleteRole()
    {
        var athleteClient = CreateClientAsRole("Athlete");
        var enrollmentId = Guid.NewGuid();

        var body = new
        {
            certificateType = "Completion",
            fileUrl = "https://example.com/cert.pdf"
        };

        var response = await PostJsonAsync(athleteClient, $"{EnrollmentsBaseRoute}/{enrollmentId}/certificate", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Complete Milestone

    [Fact]
    public async Task CompleteMilestone_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (programId, _, enrollmentId) = await CreateEnrollmentFixtureAsync(adminClient);

        await SeedMilestoneAsync(programId);
        var milestoneId = await GetMilestoneIdAsync(programId);
        Assert.NotEqual(Guid.Empty, milestoneId);

        var response = await PostAsync(adminClient, $"{ProgressBaseRoute}/enrollment/{enrollmentId}/milestones/{milestoneId}/complete");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
    }

    #endregion
}