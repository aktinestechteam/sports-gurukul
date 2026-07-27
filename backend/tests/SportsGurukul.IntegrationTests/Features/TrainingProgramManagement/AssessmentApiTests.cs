using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class AssessmentApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    private const string AssessmentsBaseRoute = "api/v1/assessments";
    private const string SessionsBaseRoute = "api/v1/training-sessions";
    private const string ProgramsBaseRoute = "api/v1/training-programs";
    private const string BatchesBaseRoute = "api/v1/training-batches";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AssessmentApiTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region Helpers

    private async Task<Guid> CreateProgramAsync(HttpClient client)
    {
        var body = new
        {
            ProgramName = $"Test Program {Guid.NewGuid().ToString()[..6]}",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Test program for assessment integration tests",
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

    private async Task<Guid> CreateSessionAsync(HttpClient client, Guid batchId)
    {
        var body = new
        {
            SessionTitle = "Assessment Session",
            SessionType = "Practice",
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };

        var response = await PostJsonAsync(client, $"{BatchesBaseRoute}/{batchId}/sessions", body);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task<Guid> CreateAssessmentAsync(HttpClient client, Guid sessionId)
    {
        var body = new
        {
            assessmentType = "SkillTest",
            assessmentName = "Mid-term Evaluation",
            maximumScore = 100,
            passingScore = 50
        };

        var response = await PostJsonAsync(client, $"{SessionsBaseRoute}/{sessionId}/assessments", body);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AssessmentDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task<(Guid ProgramId, Guid BatchId, Guid SessionId)> CreateFullFixtureAsync(HttpClient client)
    {
        var programId = await CreateProgramAsync(client);
        var batchId = await CreateBatchAsync(client, programId);
        await StartBatchAsync(client, batchId);
        var sessionId = await CreateSessionAsync(client, batchId);
        return (programId, batchId, sessionId);
    }

    #endregion

    #region Create Assessment

    [Fact]
    public async Task CreateAssessment_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var body = new
        {
            assessmentType = "SkillTest",
            assessmentName = "Mid-term Evaluation",
            maximumScore = 100,
            passingScore = 50
        };

        var response = await PostJsonAsync(adminClient, $"{SessionsBaseRoute}/{sessionId}/assessments", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AssessmentDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("SkillTest", content.Data!.AssessmentType);
        Assert.Equal("Mid-term Evaluation", content.Data.AssessmentName);
        Assert.Equal(100, content.Data.MaximumScore);
        Assert.Equal(50, content.Data.PassingScore);
        Assert.Equal(sessionId, content.Data.SessionId);
        Assert.NotEqual(Guid.Empty, content.Data.Id);
    }

    [Fact]
    public async Task CreateAssessment_Returns401_WhenAnonymous()
    {
        var anonymousClient = CreateAnonymousClient();
        var sessionId = Guid.NewGuid();

        var body = new
        {
            assessmentType = "SkillTest",
            assessmentName = "Mid-term Evaluation",
            maximumScore = 100,
            passingScore = 50
        };

        var response = await PostJsonAsync(anonymousClient, $"{SessionsBaseRoute}/{sessionId}/assessments", body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAssessment_Returns403_WhenAthleteRole()
    {
        var athleteClient = CreateClientAsRole("Athlete");
        var sessionId = Guid.NewGuid();

        var body = new
        {
            assessmentType = "SkillTest",
            assessmentName = "Mid-term Evaluation",
            maximumScore = 100,
            passingScore = 50
        };

        var response = await PostJsonAsync(athleteClient, $"{SessionsBaseRoute}/{sessionId}/assessments", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateAssessment_Returns400_WhenInvalidScore()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);

        var body = new
        {
            assessmentType = "SkillTest",
            assessmentName = "Invalid Score Assessment",
            maximumScore = 100,
            passingScore = 150
        };

        var response = await PostJsonAsync(adminClient, $"{SessionsBaseRoute}/{sessionId}/assessments", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Submit Assessment Result

    [Fact]
    public async Task SubmitAssessmentResult_ReturnsCreated_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        var assessmentId = await CreateAssessmentAsync(adminClient, sessionId);

        var body = new
        {
            athleteId = TestIds.AthleteEntityId,
            score = 85,
            remarks = "Good performance"
        };

        var response = await PostJsonAsync(adminClient, $"{AssessmentsBaseRoute}/{assessmentId}/results", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AssessmentResultDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(TestIds.AthleteEntityId, content.Data!.AthleteId);
        Assert.Equal(assessmentId, content.Data.AssessmentId);
        Assert.Equal(85, content.Data.Score);
        Assert.Equal("Good performance", content.Data.Remarks);
        Assert.True(content.Data.IsPassed);
    }

    [Fact]
    public async Task SubmitAssessmentResult_Returns409_WhenResultAlreadyExists()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        var assessmentId = await CreateAssessmentAsync(adminClient, sessionId);

        var body = new
        {
            athleteId = TestIds.AthleteEntityId,
            score = 85,
            remarks = "Good performance"
        };

        var firstResponse = await PostJsonAsync(adminClient, $"{AssessmentsBaseRoute}/{assessmentId}/results", body);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await PostJsonAsync(adminClient, $"{AssessmentsBaseRoute}/{assessmentId}/results", body);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    #endregion

    #region Publish Results

    [Fact]
    public async Task PublishResults_ReturnsOk_WhenValid()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        var assessmentId = await CreateAssessmentAsync(adminClient, sessionId);

        var resultBody = new
        {
            athleteId = TestIds.AthleteEntityId,
            score = 85,
            remarks = "Good performance"
        };
        var resultResponse = await PostJsonAsync(adminClient, $"{AssessmentsBaseRoute}/{assessmentId}/results", resultBody);
        resultResponse.EnsureSuccessStatusCode();

        var response = await PostAsync(adminClient, $"{AssessmentsBaseRoute}/{assessmentId}/publish");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
    }

    #endregion

    #region Get Assessments By Session

    [Fact]
    public async Task GetAssessmentsBySession_ReturnsOk_WhenHasAssessments()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullFixtureAsync(adminClient);
        await CreateAssessmentAsync(adminClient, sessionId);

        var response = await GetAsync(adminClient, $"{SessionsBaseRoute}/{sessionId}/assessments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AssessmentDto>>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.NotEmpty(content.Data);
        Assert.Contains(content.Data, a => a.AssessmentName == "Mid-term Evaluation");
    }

    #endregion
}