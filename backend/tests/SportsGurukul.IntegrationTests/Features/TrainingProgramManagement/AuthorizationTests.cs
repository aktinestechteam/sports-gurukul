using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class AuthorizationTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthorizationTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region Helpers

    private static object CreateProgramBody(string name)
    {
        return new
        {
            ProgramName = name,
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Test program for authorization tests",
            DifficultyLevel = 0,
            MinimumAge = 8,
            MaximumAge = 18,
            DurationWeeks = 12,
            Capacity = 30
        };
    }

    private async Task<Guid> CreateProgramAsync(HttpClient client, string? name = null)
    {
        var body = CreateProgramBody(name ?? $"Auth Test Program {Guid.NewGuid().ToString()[..8]}");
        var response = await PostJsonAsync(client, "/api/v1/training-programs", body);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
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
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task<Guid> CreateSessionAsync(HttpClient client, Guid batchId)
    {
        var sessionRequest = new
        {
            SessionTitle = "Test Session",
            SessionType = SessionType.Practice,
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };
        var response = await PostJsonAsync(client, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task<(Guid programId, Guid batchId)> CreatePrerequisitesAsync(HttpClient adminClient)
    {
        var programId = await CreateProgramAsync(adminClient);
        var batchId = await CreateBatchAsync(adminClient, programId);
        return (programId, batchId);
    }

    private async Task<(Guid programId, Guid batchId, Guid sessionId)> CreateFullPrerequisitesAsync(HttpClient adminClient)
    {
        var (programId, batchId) = await CreatePrerequisitesAsync(adminClient);
        var sessionId = await CreateSessionAsync(adminClient, batchId);
        return (programId, batchId, sessionId);
    }

    private async Task ActivateBatchAsync(HttpClient adminClient, Guid batchId)
    {
        var response = await PostAsync(adminClient, $"/api/v1/training-batches/{batchId}/start");
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region 1. Create Program

    [Fact]
    public async Task CreateProgram_401_Anonymous()
    {
        var client = CreateAnonymousClient();
        var body = CreateProgramBody("Anonymous Program");
        var response = await PostJsonAsync(client, "/api/v1/training-programs", body);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProgram_403_Athlete()
    {
        var client = CreateClientAsRole("Athlete");
        var body = CreateProgramBody("Athlete Program");
        var response = await PostJsonAsync(client, "/api/v1/training-programs", body);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProgram_201_AcademyAdmin()
    {
        var client = CreateClientAsRole("Academy Admin");
        var body = CreateProgramBody("Academy Admin Program");
        var response = await PostJsonAsync(client, "/api/v1/training-programs", body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 2. Update Program

    [Fact]
    public async Task UpdateProgram_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var anonClient = CreateAnonymousClient();
        var updateBody = new { programName = "Updated Name" };
        var response = await PutJsonAsync(anonClient, $"/api/v1/training-programs/{programId}", updateBody);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProgram_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var updateBody = new { programName = "Athlete Updated Name" };
        var response = await PutJsonAsync(athleteClient, $"/api/v1/training-programs/{programId}", updateBody);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProgram_200_AcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var academyAdminClient = CreateClientAsRole("Academy Admin");
        var updateBody = new { programName = "Academy Admin Updated" };
        var response = await PutJsonAsync(academyAdminClient, $"/api/v1/training-programs/{programId}", updateBody);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.True(content!.Success);
        Assert.Equal("Academy Admin Updated", content.Data!.ProgramName);
    }

    #endregion

    #region 3. Delete Program

    [Fact]
    public async Task DeleteProgram_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var anonClient = CreateAnonymousClient();
        var response = await DeleteAsync(anonClient, $"/api/v1/training-programs/{programId}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProgram_403_AcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var academyAdminClient = CreateClientAsRole("Academy Admin");
        var response = await DeleteAsync(academyAdminClient, $"/api/v1/training-programs/{programId}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProgram_200_SystemAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var response = await DeleteAsync(adminClient, $"/api/v1/training-programs/{programId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 4. Publish Program

    [Fact]
    public async Task PublishProgram_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var anonClient = CreateAnonymousClient();
        var response = await PostAsync(anonClient, $"/api/v1/training-programs/{programId}/publish");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PublishProgram_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var response = await PostAsync(athleteClient, $"/api/v1/training-programs/{programId}/publish");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PublishProgram_200_AcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient, "Publishable Program");

        var academyAdminClient = CreateClientAsRole("Academy Admin");
        var response = await PostAsync(academyAdminClient, $"/api/v1/training-programs/{programId}/publish");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 5. Archive Program

    [Fact]
    public async Task ArchiveProgram_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        await PostAsync(adminClient, $"/api/v1/training-programs/{programId}/publish");

        var anonClient = CreateAnonymousClient();
        var response = await PostAsync(anonClient, $"/api/v1/training-programs/{programId}/archive");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ArchiveProgram_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);
        await PostAsync(adminClient, $"/api/v1/training-programs/{programId}/publish");

        var athleteClient = CreateClientAsRole("Athlete");
        var response = await PostAsync(athleteClient, $"/api/v1/training-programs/{programId}/archive");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ArchiveProgram_200_AcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient, "Archivable Program");

        await PostAsync(adminClient, $"/api/v1/training-programs/{programId}/publish");

        var academyAdminClient = CreateClientAsRole("Academy Admin");
        var response = await PostAsync(academyAdminClient, $"/api/v1/training-programs/{programId}/archive");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 6. Create Batch

    [Fact]
    public async Task CreateBatch_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var anonClient = CreateAnonymousClient();
        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };
        var response = await PostJsonAsync(anonClient, $"/api/v1/training-programs/{programId}/batches", batchRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBatch_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
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
    public async Task CreateBatch_201_AcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramAsync(adminClient);

        var academyAdminClient = CreateClientAsRole("Academy Admin");
        var batchRequest = new
        {
            CoachId = TestIds.CoachEntityId,
            BranchId = TestIds.AcademyBranchId,
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            MaximumSeats = 20
        };
        var response = await PostJsonAsync(academyAdminClient, $"/api/v1/training-programs/{programId}/batches", batchRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 7. Update Batch

    [Fact]
    public async Task UpdateBatch_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);

        var anonClient = CreateAnonymousClient();
        var updateBody = new { MaximumSeats = 50 };
        var response = await PutJsonAsync(anonClient, $"/api/v1/training-batches/{batchId}", updateBody);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBatch_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var updateBody = new { MaximumSeats = 50 };
        var response = await PutJsonAsync(athleteClient, $"/api/v1/training-batches/{batchId}", updateBody);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBatch_200_AcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);

        var academyAdminClient = CreateClientAsRole("Academy Admin");
        var updateBody = new { MaximumSeats = 50 };
        var response = await PutJsonAsync(academyAdminClient, $"/api/v1/training-batches/{batchId}", updateBody);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>(JsonOptions);
        Assert.True(content!.Success);
        Assert.Equal(50, content.Data!.MaximumSeats);
    }

    #endregion

    #region 8. Start Batch

    [Fact]
    public async Task StartBatch_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);

        var anonClient = CreateAnonymousClient();
        var response = await PostAsync(anonClient, $"/api/v1/training-batches/{batchId}/start");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task StartBatch_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var response = await PostAsync(athleteClient, $"/api/v1/training-batches/{batchId}/start");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task StartBatch_200_AcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);

        var academyAdminClient = CreateClientAsRole("Academy Admin");
        var response = await PostAsync(academyAdminClient, $"/api/v1/training-batches/{batchId}/start");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingBatchDto>>(JsonOptions);
        Assert.True(content!.Success);
        Assert.Equal("Active", content.Data!.Status);
    }

    #endregion

    #region 9. Create Session

    [Fact]
    public async Task CreateSession_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var anonClient = CreateAnonymousClient();
        var sessionRequest = new
        {
            SessionTitle = "Anonymous Session",
            SessionType = SessionType.Practice,
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };
        var response = await PostJsonAsync(anonClient, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateSession_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var athleteClient = CreateClientAsRole("Athlete");
        var sessionRequest = new
        {
            SessionTitle = "Athlete Session",
            SessionType = SessionType.Practice,
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
    public async Task CreateSession_201_Coach()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var coachClient = CreateClientAsRole("Coach");
        var sessionRequest = new
        {
            SessionTitle = "Coach Created Session",
            SessionType = SessionType.Practice,
            SessionDate = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            FacilityId = TestIds.FacilityId,
            CoachId = TestIds.CoachEntityId
        };
        var response = await PostJsonAsync(coachClient, $"/api/v1/training-batches/{batchId}/sessions", sessionRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 10. Update Session

    [Fact]
    public async Task UpdateSession_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullPrerequisitesAsync(adminClient);

        var anonClient = CreateAnonymousClient();
        var updateBody = new { sessionTitle = "Updated Title" };
        var response = await PutJsonAsync(anonClient, $"/api/v1/training-sessions/{sessionId}", updateBody);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSession_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullPrerequisitesAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var updateBody = new { sessionTitle = "Athlete Updated Title" };
        var response = await PutJsonAsync(athleteClient, $"/api/v1/training-sessions/{sessionId}", updateBody);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSession_200_Coach()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, _, sessionId) = await CreateFullPrerequisitesAsync(adminClient);

        var coachClient = CreateClientAsRole("Coach");
        var updateBody = new { sessionTitle = "Coach Updated Session" };
        var response = await PutJsonAsync(coachClient, $"/api/v1/training-sessions/{sessionId}", updateBody);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingSessionDto>>(JsonOptions);
        Assert.True(content!.Success);
        Assert.Equal("Coach Updated Session", content.Data!.SessionTitle);
    }

    #endregion

    #region 11. Enroll Athlete

    [Fact]
    public async Task EnrollAthlete_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var anonClient = CreateAnonymousClient();
        var enrollRequest = new { AthleteId = TestIds.AthleteEntityId };
        var response = await PostJsonAsync(anonClient, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task EnrollAthlete_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var athleteClient = CreateClientAsRole("Athlete");
        var enrollRequest = new { AthleteId = TestIds.AthleteEntityId };
        var response = await PostJsonAsync(athleteClient, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EnrollAthlete_201_Coach()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId) = await CreatePrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var coachClient = CreateClientAsRole("Coach");
        var enrollRequest = new { AthleteId = TestIds.AthleteEntityId };
        var response = await PostJsonAsync(coachClient, $"/api/v1/training-batches/{batchId}/enrollments", enrollRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EnrollmentDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 12. Mark Attendance

    [Fact]
    public async Task MarkAttendance_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId, sessionId) = await CreateFullPrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var anonClient = CreateAnonymousClient();
        var attendanceRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "Test attendance"
        };
        var response = await PostJsonAsync(anonClient, $"/api/v1/training-sessions/{sessionId}/attendance", attendanceRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MarkAttendance_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId, sessionId) = await CreateFullPrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var athleteClient = CreateClientAsRole("Athlete");
        var attendanceRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "Test attendance"
        };
        var response = await PostJsonAsync(athleteClient, $"/api/v1/training-sessions/{sessionId}/attendance", attendanceRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task MarkAttendance_201_Coach()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId, sessionId) = await CreateFullPrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var coachClient = CreateClientAsRole("Coach");
        var attendanceRequest = new
        {
            AthleteId = TestIds.AthleteEntityId,
            Status = AttendanceStatus.Present,
            Remarks = "Coach recorded attendance"
        };
        var response = await PostJsonAsync(coachClient, $"/api/v1/training-sessions/{sessionId}/attendance", attendanceRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion

    #region 13. Create Assessment

    [Fact]
    public async Task CreateAssessment_401_Anonymous()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId, sessionId) = await CreateFullPrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var anonClient = CreateAnonymousClient();
        var assessmentRequest = new
        {
            AssessmentType = "SkillTest",
            AssessmentName = "Anonymous Assessment",
            MaximumScore = 100m,
            PassingScore = 50m
        };
        var response = await PostJsonAsync(anonClient, $"/api/v1/training-sessions/{sessionId}/assessments", assessmentRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAssessment_403_Athlete()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId, sessionId) = await CreateFullPrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var athleteClient = CreateClientAsRole("Athlete");
        var assessmentRequest = new
        {
            AssessmentType = "SkillTest",
            AssessmentName = "Athlete Assessment",
            MaximumScore = 100m,
            PassingScore = 50m
        };
        var response = await PostJsonAsync(athleteClient, $"/api/v1/training-sessions/{sessionId}/assessments", assessmentRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateAssessment_201_Coach()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var (_, batchId, sessionId) = await CreateFullPrerequisitesAsync(adminClient);
        await ActivateBatchAsync(adminClient, batchId);

        var coachClient = CreateClientAsRole("Coach");
        var assessmentRequest = new
        {
            AssessmentType = "SkillTest",
            AssessmentName = "Coach Created Assessment",
            MaximumScore = 100m,
            PassingScore = 50m
        };
        var response = await PostJsonAsync(coachClient, $"/api/v1/training-sessions/{sessionId}/assessments", assessmentRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AssessmentDto>>(JsonOptions);
        Assert.True(content!.Success);
    }

    #endregion
}