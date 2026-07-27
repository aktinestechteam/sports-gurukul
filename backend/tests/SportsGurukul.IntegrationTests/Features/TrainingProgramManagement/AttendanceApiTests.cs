using System.Net;
using System.Net.Http.Json;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class AttendanceApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    public AttendanceApiTests(CustomWebApplicationFactory factory) : base(factory) { }

    private async Task<Guid> CreateProgramAsync(HttpClient client)
    {
        var programRequest = new
        {
            ProgramName = $"Test Program {Guid.NewGuid().ToString()[..6]}",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Test training program for attendance integration tests",
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

    private async Task EnrollAthleteAsync(HttpClient client, Guid batchId)
    {
        var enrollRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(client, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);
        response.EnsureSuccessStatusCode();
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

    private async Task<(Guid ProgramId, Guid BatchId, Guid SessionId)> CreateFullFixtureAsync(HttpClient adminClient, HttpClient? coachClient = null)
    {
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);
        await StartBatchAsync(adminClient, batchId);
        await EnrollAthleteAsync(adminClient, batchId);
        var sessionId = await CreateSessionAsync(adminClient, batchId);
        return (programId, batchId, sessionId);
    }

    private async Task<Guid> MarkAttendanceAsync(HttpClient client, Guid sessionId)
    {
        var markRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "On time"
        };

        var response = await PostJsonAsync(client, $"/api/v1/training-sessions/{sessionId}/attendance", markRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceDto>>();
        return content!.Data!.Id;
    }

    [Fact]
    public async Task MarkAttendance_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var coachClient = CreateClientAsRole("Coach");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var markRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "On time"
        };

        var response = await PostJsonAsync(coachClient, $"/api/v1/training-sessions/{sessionId}/attendance", markRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(sessionId, content.Data!.SessionId);
        Assert.Equal(TestIds.AthleteEntityId, content.Data.AthleteId);
        Assert.Equal("Present", content.Data.AttendanceStatus);
        Assert.Equal("On time", content.Data.Remarks);
    }

    [Fact]
    public async Task MarkAttendance_Returns401_WhenAnonymous()
    {
        var anonymousClient = CreateAnonymousClient();
        var sessionId = Guid.NewGuid();

        var markRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "On time"
        };

        var response = await PostJsonAsync(anonymousClient, $"/api/v1/training-sessions/{sessionId}/attendance", markRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MarkAttendance_Returns403_WhenAthleteRole()
    {
        var athleteClient = CreateClientAsRole("Athlete");
        var sessionId = Guid.NewGuid();

        var markRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "On time"
        };

        var response = await PostJsonAsync(athleteClient, $"/api/v1/training-sessions/{sessionId}/attendance", markRequest);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task MarkAttendance_Returns409_WhenDuplicate()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var coachClient = CreateClientAsRole("Coach");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var firstMarkRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "On time"
        };

        var firstResponse = await PostJsonAsync(coachClient, $"/api/v1/training-sessions/{sessionId}/attendance", firstMarkRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondMarkRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "Already marked"
        };

        var secondResponse = await PostJsonAsync(coachClient, $"/api/v1/training-sessions/{sessionId}/attendance", secondMarkRequest);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task GetSessionAttendance_ReturnsOk_WhenHasRecords()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var coachClient = CreateClientAsRole("Coach");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        await MarkAttendanceAsync(coachClient, sessionId);

        var response = await GetAsync(adminClient, $"/api/v1/training-sessions/{sessionId}/attendance");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AttendanceDto>>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Single(content.Data);
        Assert.Equal(TestIds.AthleteEntityId, content.Data[0].AthleteId);
        Assert.Equal(sessionId, content.Data[0].SessionId);
    }

    [Fact]
    public async Task CheckIn_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var coachClient = CreateClientAsRole("Coach");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        await MarkAttendanceAsync(coachClient, sessionId);

        var checkInRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-sessions/{sessionId}/check-in", checkInRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(sessionId, content.Data!.SessionId);
        Assert.Equal(TestIds.AthleteEntityId, content.Data.AthleteId);
        Assert.NotNull(content.Data.CheckInTime);
    }

    [Fact]
    public async Task CheckOut_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var coachClient = CreateClientAsRole("Coach");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        await MarkAttendanceAsync(coachClient, sessionId);

        var checkInRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var checkInResponse = await PostJsonAsync(adminClient, $"/api/v1/training-sessions/{sessionId}/check-in", checkInRequest);
        checkInResponse.EnsureSuccessStatusCode();

        var checkOutRequest = new
        {
            AthleteId = TestIds.AthleteEntityId
        };

        var response = await PostJsonAsync(adminClient, $"/api/v1/training-sessions/{sessionId}/check-out", checkOutRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(sessionId, content.Data!.SessionId);
        Assert.Equal(TestIds.AthleteEntityId, content.Data.AthleteId);
        Assert.NotNull(content.Data.CheckInTime);
        Assert.NotNull(content.Data.CheckOutTime);
    }

    [Fact]
    public async Task UpdateAttendance_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var coachClient = CreateClientAsRole("Coach");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        var attendanceId = await MarkAttendanceAsync(coachClient, sessionId);

        var updateRequest = new
        {
            Status = AttendanceStatus.Late,
            Remarks = "Arrived 15 minutes late"
        };

        var response = await PutJsonAsync(adminClient, $"/api/v1/attendance/{attendanceId}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceDto>>();
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(attendanceId, content.Data!.Id);
        Assert.Equal("Late", content.Data.AttendanceStatus);
        Assert.Equal("Arrived 15 minutes late", content.Data.Remarks);
    }
}