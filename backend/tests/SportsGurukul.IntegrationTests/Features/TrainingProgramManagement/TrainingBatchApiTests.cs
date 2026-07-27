using System.Net;
using System.Net.Http.Json;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class TrainingBatchApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    public TrainingBatchApiTests(CustomWebApplicationFactory factory) : base(factory) { }

    private async Task<Guid> CreateProgramAsync(HttpClient client)
    {
        var programRequest = new
        {
            ProgramName = $"Test Program {Guid.NewGuid().ToString()[..6]}",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Test training program for batch integration tests",
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

    [Fact]
    public async Task CreateBatch_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-programs/{programId}/batches", batchRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(TestIds.CoachEntityId, content.Data!.CoachId);
        Assert.Equal(TestIds.AcademyBranchId, content.Data.BranchId);
        Assert.Equal(20, content.Data.MaximumSeats);
        Assert.Equal("Inactive", content.Data.Status);
    }

    [Fact]
    public async Task CreateBatch_Returns401_WhenAnonymous()
    {
        var anonymousClient = CreateAnonymousClient();
        var programId = Guid.NewGuid();

        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };

        var response = await PostJsonAsync(anonymousClient, $"/api/v1/training-programs/{programId}/batches", batchRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBatch_Returns403_WhenAthlete()
    {
        var athleteClient = CreateClientAsRole("Athlete");
        var programId = Guid.NewGuid();

        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };

        var response = await PostJsonAsync(athleteClient, $"/api/v1/training-programs/{programId}/batches", batchRequest);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateBatch_Returns404_WhenProgramNotFound()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var nonExistentProgramId = Guid.NewGuid();

        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-programs/{nonExistentProgramId}/batches", batchRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBatch_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);

        var updateRequest = new
        {
            StartDate = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 11, 30, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 25
        };

        var response = await PutJsonAsync(adminClient, $"/api/v1/training-batches/{batchId}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(25, content.Data!.MaximumSeats);
        Assert.Equal(new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc), content.Data.StartDate);
    }

    [Fact]
    public async Task StartBatch_ReturnsOk_WhenActiveBatch()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);

        var response = await PostAsync(adminClient, $"/api/v1/training-batches/{batchId}/start");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("Active", content.Data!.Status);
    }

    [Fact]
    public async Task CompleteBatch_ReturnsOk_WhenStartedBatch()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);

        var startResponse = await PostAsync(adminClient, $"/api/v1/training-batches/{batchId}/start");
        startResponse.EnsureSuccessStatusCode();

        var response = await PostAsync(adminClient, $"/api/v1/training-batches/{batchId}/complete");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("Completed", content.Data!.Status);
    }

    [Fact]
    public async Task CancelBatch_ReturnsOk_WhenActiveBatch()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);

        var response = await PostAsync(adminClient, $"/api/v1/training-batches/{batchId}/cancel");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("Cancelled", content.Data!.Status);
    }

    [Fact]
    public async Task GetBatch_ReturnsOk_WhenExists()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);

        var response = await GetAsync(adminClient, $"/api/v1/training-batches/{batchId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(batchId, content.Data!.Id);
        Assert.Equal(TestIds.CoachEntityId, content.Data.CoachId);
        Assert.Equal(TestIds.AcademyBranchId, content.Data.BranchId);
    }

    [Fact]
    public async Task GetBatchesByProgram_ReturnsOk_WhenProgramHasBatches()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        await CreateBatchAsync(adminClient, programId);
        await CreateBatchAsync(adminClient, programId);

        var response = await GetAsync(adminClient, $"/api/v1/training-programs/{programId}/batches");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<TrainingBatchDto>>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(2, content.Data.Count);
    }

    [Fact]
    public async Task CreateBatch_Returns404_WhenProgramDoesNotExist()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var nonExistentProgramId = Guid.NewGuid();

        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-programs/{nonExistentProgramId}/batches", batchRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}