using System.Net;
using System.Net.Http.Json;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class EnrollmentApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    public EnrollmentApiTests(CustomWebApplicationFactory factory) : base(factory) { }

    private async Task<Guid> CreateProgramAsync(HttpClient client)
    {
        var programRequest = new
        {
            ProgramName = $"Test Program {Guid.NewGuid().ToString()[..6]}",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Test training program for enrollment integration tests",
            DifficultyLevel = 0,
            MinimumAge = 8,
            MaximumAge = 18,
            DurationWeeks = 24,
            Capacity = 30
        };

        var response = await PostJsonAsync(client, "/api/v1/training-programs", programRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>();
        return content!.Data!.Id;
    }

    private async Task<Guid> CreateBatchAsync(HttpClient client, Guid programId)
    {
        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };

        var response = await PostJsonAsync(client, $"/api/v1/training-programs/{programId}/batches", batchRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        return content!.Data!.Id;
    }

    private async Task<Guid> StartBatchAsync(HttpClient client, Guid batchId)
    {
        var response = await PostAsync(client, $"/api/v1/training-batches/{batchId}/start");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        return content!.Data!.Id;
    }

    private async Task<Guid> EnrollAthleteAsync(HttpClient client, Guid batchId)
    {
        var enrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(client, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EnrollmentDto>>();
        return content!.Data!.Id;
    }

    [Fact]
    public async Task EnrollAthlete_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);
        await StartBatchAsync(adminClient, batchId);

        var enrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EnrollmentDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(batchId, content.Data!.BatchId);
        Assert.Equal(TestIds.AthleteEntityId, content.Data.AthleteId);
        Assert.Equal("Active", content.Data.Status);
    }

    [Fact]
    public async Task EnrollAthlete_Returns401_WhenAnonymous()
    {
        var anonymousClient = CreateAnonymousClient();
        var batchId = Guid.NewGuid();

        var enrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(anonymousClient, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task EnrollAthlete_Returns403_WhenAthleteRole()
    {
        var athleteClient = CreateClientAsRole("Athlete");
        var batchId = Guid.NewGuid();

        var enrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(athleteClient, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EnrollAthlete_Returns409_WhenAlreadyEnrolled()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);
        await StartBatchAsync(adminClient, batchId);

        var firstEnrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var firstResponse = await PostJsonAsync(adminClient, $"/api/v1/training-batches/{batchId}/enrollments", firstEnrollRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondEnrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var secondResponse = await PostJsonAsync(adminClient, $"/api/v1/training-batches/{batchId}/enrollments", secondEnrollRequest);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task GetEnrollmentsByBatch_ReturnsOk_WhenBatchHasEnrollments()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);
        await StartBatchAsync(adminClient, batchId);
        await EnrollAthleteAsync(adminClient, batchId);

        var response = await GetAsync(adminClient, $"/api/v1/training-batches/{batchId}/enrollments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<EnrollmentDto>>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Single(content.Data);
        Assert.Equal(TestIds.AthleteEntityId, content.Data[0].AthleteId);
        Assert.Equal(batchId, content.Data[0].BatchId);
    }

    [Fact]
    public async Task CancelEnrollment_ReturnsOk_WhenActive()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);
        await StartBatchAsync(adminClient, batchId);
        var enrollmentId = await EnrollAthleteAsync(adminClient, batchId);

        var response = await DeleteAsync(adminClient, $"/api/v1/enrollments/{enrollmentId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EnrollmentDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(enrollmentId, content.Data!.Id);
        Assert.Equal("Withdrawn", content.Data.Status);
    }

    [Fact]
    public async Task TransferEnrollment_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var sourceBatchId = await CreateBatchAsync(adminClient, programId);
        var targetBatchId = await CreateBatchAsync(adminClient, programId);
        await StartBatchAsync(adminClient, sourceBatchId);
        await StartBatchAsync(adminClient, targetBatchId);
        var enrollmentId = await EnrollAthleteAsync(adminClient, sourceBatchId);

        var transferRequest = new
        {
            SourceBatchId = sourceBatchId,
            TargetBatchId = targetBatchId
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/enrollments/{enrollmentId}/transfer", transferRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EnrollmentDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(enrollmentId, content.Data!.Id);
        Assert.Equal(targetBatchId, content.Data.BatchId);
        Assert.Equal("Active", content.Data.Status);
    }

    [Fact]
    public async Task CompleteEnrollment_ReturnsOk_WhenActive()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);
        await StartBatchAsync(adminClient, batchId);
        var enrollmentId = await EnrollAthleteAsync(adminClient, batchId);

        var response = await PostAsync(adminClient, $"/api/v1/enrollments/{enrollmentId}/complete");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EnrollmentDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(enrollmentId, content.Data!.Id);
        Assert.Equal("Completed", content.Data.Status);
    }

    [Fact]
    public async Task EnrollAthlete_Returns404_WhenBatchNotFound()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var nonExistentBatchId = Guid.NewGuid();

        var enrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-batches/{nonExistentBatchId}/enrollments", enrollRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}