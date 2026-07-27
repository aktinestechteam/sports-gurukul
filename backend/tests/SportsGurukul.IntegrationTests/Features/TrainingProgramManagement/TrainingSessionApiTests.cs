using System.Net;
using System.Net.Http.Json;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class TrainingSessionApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    public TrainingSessionApiTests(CustomWebApplicationFactory factory) : base(factory) { }

    private async Task<Guid> CreateProgramAsync(HttpClient client)
    {
        var programRequest = new
        {
            ProgramName = $"Test Program {Guid.NewGuid().ToString()[..6]}",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Test training program for session integration tests",
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

    private async Task<Guid> CreateSessionAsync(HttpClient client, Guid batchId)
    {
        var sessionRequest = new
        {
            SessionTitle = "Morning Practice",
            SessionType = "Practice",
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };

        var response = await PostJsonAsync(client, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>();
        return content!.Data!.Id;
    }

    private async Task<(Guid ProgramId, Guid BatchId, Guid SessionId)> CreateFullFixtureAsync(HttpClient client)
    {
        var programId = await CreateProgramAsync(client);
        var batchId = await CreateBatchAsync(client, programId);
        var sessionId = await CreateSessionAsync(client, batchId);
        return (programId, batchId, sessionId);
    }

    [Fact]
    public async Task CreateSession_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);

        var sessionRequest = new
        {
            SessionTitle = "Morning Practice",
            SessionType = "Practice",
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("Morning Practice", content.Data!.SessionTitle);
        Assert.Equal("Practice", content.Data.SessionType);
        Assert.Equal(TestIds.CoachEntityId, content.Data.CoachId);
        Assert.Equal(TestIds.FacilityId, content.Data.FacilityId);
        Assert.Equal(batchId, content.Data.BatchId);
    }

    [Fact]
    public async Task CreateSession_Returns401_WhenAnonymous()
    {
        var anonymousClient = CreateAnonymousClient();
        var batchId = Guid.NewGuid();

        var sessionRequest = new
        {
            SessionTitle = "Morning Practice",
            SessionType = "Practice",
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };

        var response = await PostJsonAsync(anonymousClient, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateSession_Returns403_WhenAthlete()
    {
        var athleteClient = CreateClientAsRole("Athlete");
        var batchId = Guid.NewGuid();

        var sessionRequest = new
        {
            SessionTitle = "Morning Practice",
            SessionType = "Practice",
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };

        var response = await PostJsonAsync(athleteClient, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSession_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var updateRequest = new
        {
            SessionTitle = "Updated Practice Session",
            SessionType = "Practice",
            SessionDate = new DateTime(2026, 8, 6, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0)
        };

        var response = await PutJsonAsync(adminClient, $"/api/v1/training-sessions/{sessionId}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("Updated Practice Session", content.Data!.SessionTitle);
        Assert.Equal(new DateTime(2026, 8, 6, 0, 0, 0, DateTimeKind.Utc), content.Data.SessionDate);
        Assert.Equal(new TimeSpan(10, 0, 0), content.Data.StartTime);
        Assert.Equal(new TimeSpan(12, 0, 0), content.Data.EndTime);
    }

    [Fact]
    public async Task RescheduleSession_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var rescheduleRequest = new
        {
            SessionDate = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0)
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-sessions/{sessionId}/reschedule", rescheduleRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc), content.Data!.SessionDate);
        Assert.Equal(new TimeSpan(14, 0, 0), content.Data.StartTime);
        Assert.Equal(new TimeSpan(16, 0, 0), content.Data.EndTime);
    }

    [Fact]
    public async Task CompleteSession_ReturnsOk_WhenScheduled()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var response = await PostAsync(adminClient, $"/api/v1/training-sessions/{sessionId}/complete");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("Completed", content.Data!.Status);
    }

    [Fact]
    public async Task GetSession_ReturnsOk_WhenExists()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var response = await GetAsync(adminClient, $"/api/v1/training-sessions/{sessionId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(sessionId, content.Data!.Id);
        Assert.Equal("Morning Practice", content.Data.SessionTitle);
        Assert.Equal("Practice", content.Data.SessionType);
        Assert.Equal(TestIds.CoachEntityId, content.Data.CoachId);
    }

    [Fact]
    public async Task GetSessionsByBatch_ReturnsOk_WhenBatchHasSessions()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId, _) = await CreateFullFixtureAsync(adminClient);

        var sessionRequest2 = new
        {
            SessionTitle = "Afternoon Theory",
            SessionType = "Theory",
            SessionDate = new DateTime(2026, 8, 6, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };
        var createResponse = await PostJsonAsync(adminClient, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest2);
        createResponse.EnsureSuccessStatusCode();

        var response = await GetAsync(adminClient, $"/api/v1/training-batches/{batchId}/sessions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<TrainingSessionDto>>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(2, content.Data.Count);
        Assert.Contains(content.Data, s => s.SessionTitle == "Morning Practice");
        Assert.Contains(content.Data, s => s.SessionTitle == "Afternoon Theory");
    }

    [Fact]
    public async Task GetUpcomingSessions_ReturnsOk()
    {
        var response = await GetAsync(HttpClient, "/api/v1/training-sessions/upcoming");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<TrainingSessionDto>>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
    }
}