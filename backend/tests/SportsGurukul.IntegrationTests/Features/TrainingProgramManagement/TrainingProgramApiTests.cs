using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class TrainingProgramApiTests : TestBase
{
    private const string BaseRoute = "api/v1/training-programs";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TrainingProgramApiTests(CustomWebApplicationFactory factory) : base(factory)
    {
        SeedTestDataAsync().GetAwaiter().GetResult();
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await dbContext.Academies.AnyAsync(a => a.Id == TestIds.AcademyId))
            return;

        var sportCategory = new SportCategory
        {
            Id = TestIds.SportCategoryId,
            Name = TestConstants.SportCategoryName,
            Description = "Test team sports category"
        };
        dbContext.SportCategories.Add(sportCategory);

        var sport = new Sport
        {
            Id = TestIds.SportId,
            Name = TestConstants.SportName,
            Code = TestConstants.SportCode,
            SportCategoryId = TestIds.SportCategoryId,
            OlympicSport = true,
            Description = "Test cricket sport"
        };
        dbContext.Sports.Add(sport);

        var academy = new Academy
        {
            Id = TestIds.AcademyId,
            AcademyCode = TestConstants.AcademyCode,
            Name = TestConstants.AcademyName,
            Email = TestConstants.AcademyEmail,
            Phone = TestConstants.AcademyPhone,
            Status = AcademyStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            Description = "Test academy for integration tests"
        };
        dbContext.Academies.Add(academy);

        var systemAdminRole = new Role
        {
            Id = TestIds.SystemAdminRoleId,
            Name = TestConstants.SystemAdminRoleName,
            RoleType = RoleType.SuperAdmin,
            Description = "System administrator with full access"
        };

        var academyAdminRole = new Role
        {
            Id = TestIds.AcademyAdminRoleId,
            Name = TestConstants.AcademyAdminRoleName,
            RoleType = RoleType.Academy,
            Description = "Academy administrator"
        };

        var coachRole = new Role
        {
            Id = TestIds.CoachRoleId,
            Name = TestConstants.CoachRoleName,
            RoleType = RoleType.Coach,
            Description = "Coach role"
        };

        var athleteRole = new Role
        {
            Id = TestIds.AthleteRoleId,
            Name = TestConstants.AthleteRoleName,
            RoleType = RoleType.Athlete,
            Description = "Athlete role"
        };
        dbContext.Roles.AddRange(systemAdminRole, academyAdminRole, coachRole, athleteRole);

        var systemAdminUser = new User
        {
            Id = TestIds.SystemAdminUserId,
            FullName = TestConstants.SystemAdminName,
            Email = TestConstants.SystemAdminEmail,
            PhoneNumber = "+919200000001",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true
        };

        var academyAdminUser = new User
        {
            Id = TestIds.AcademyAdminUserId,
            FullName = TestConstants.AcademyAdminName,
            Email = TestConstants.AcademyAdminEmail,
            PhoneNumber = "+919200000002",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true
        };

        var coachUser = new User
        {
            Id = TestIds.CoachUserId,
            FullName = TestConstants.CoachName,
            Email = TestConstants.CoachEmail,
            PhoneNumber = "+919200000003",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true
        };

        var athleteUser = new User
        {
            Id = TestIds.AthleteUserId,
            FullName = TestConstants.AthleteName,
            Email = TestConstants.AthleteEmail,
            PhoneNumber = "+919200000004",
            PasswordHash = "test_password_hash",
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            IsEmailVerified = true
        };
        dbContext.Users.AddRange(systemAdminUser, academyAdminUser, coachUser, athleteUser);

        dbContext.UserRoles.AddRange(
            new UserRole { UserId = TestIds.SystemAdminUserId, RoleId = TestIds.SystemAdminRoleId },
            new UserRole { UserId = TestIds.AcademyAdminUserId, RoleId = TestIds.AcademyAdminRoleId },
            new UserRole { UserId = TestIds.CoachUserId, RoleId = TestIds.CoachRoleId },
            new UserRole { UserId = TestIds.AthleteUserId, RoleId = TestIds.AthleteRoleId }
        );

        await dbContext.SaveChangesAsync();
    }

    #region Helpers

    private static object CreateValidProgramBody(string programName = "Test Program")
    {
        return new
        {
            academyId = TestIds.AcademyId,
            sportId = TestIds.SportId,
            programName,
            difficultyLevel = "Beginner",
            minimumAge = 8,
            maximumAge = 16,
            durationWeeks = 12,
            capacity = 30
        };
    }

    private async Task<Guid> CreateProgramViaApiAsync(HttpClient client, string programName = "Test Program")
    {
        var body = CreateValidProgramBody(programName);
        var response = await PostJsonAsync(client, BaseRoute, body);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        return content!.Data!.Id;
    }

    private async Task<TrainingProgramDto?> GetProgramFromApiAsync(HttpClient client, Guid programId)
    {
        var response = await GetAsync(client, $"{BaseRoute}/{programId}");
        if (response.StatusCode != HttpStatusCode.OK)
            return null;
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        return content?.Data;
    }

    #endregion

    #region Create Program

    [Fact]
    public async Task CreateProgram_ReturnsCreated_WhenValidRequest()
    {
        var client = CreateClientAsRole("System Admin");
        var body = CreateValidProgramBody("Create Valid Program");

        var response = await PostJsonAsync(client, BaseRoute, body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.False(string.IsNullOrEmpty(content.Message));

        var program = content.Data!;
        Assert.Equal("Create Valid Program", program.ProgramName);
        Assert.Equal(TestIds.AcademyId, program.AcademyId);
        Assert.Equal(TestIds.SportId, program.SportId);
        Assert.Equal("Beginner", program.DifficultyLevel);
        Assert.Equal(8, program.MinimumAge);
        Assert.Equal(16, program.MaximumAge);
        Assert.Equal(12, program.DurationWeeks);
        Assert.Equal(30, program.Capacity);
        Assert.False(string.IsNullOrEmpty(program.ProgramCode));
        Assert.NotEqual(Guid.Empty, program.Id);
    }

    [Fact]
    public async Task CreateProgram_Returns401_WhenAnonymous()
    {
        var client = CreateAnonymousClient();
        var body = CreateValidProgramBody();

        var response = await PostJsonAsync(client, BaseRoute, body);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProgram_Returns403_WhenAthleteRole()
    {
        var client = CreateClientAsRole("Athlete");
        var body = CreateValidProgramBody();

        var response = await PostJsonAsync(client, BaseRoute, body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProgram_Returns400_WhenMissingRequiredFields()
    {
        var client = CreateClientAsRole("System Admin");
        var body = new { };

        var response = await PostJsonAsync(client, BaseRoute, body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProgram_Returns409_WhenDuplicateName()
    {
        var client = CreateClientAsRole("System Admin");
        var body = CreateValidProgramBody("Duplicate Name Program");

        var firstResponse = await PostJsonAsync(client, BaseRoute, body);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await PostJsonAsync(client, BaseRoute, body);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    #endregion

    #region Update Program

    [Fact]
    public async Task UpdateProgram_ReturnsOk_WhenValidRequest()
    {
        var client = CreateClientAsRole("System Admin");
        var programId = await CreateProgramViaApiAsync(client, "Program To Update");

        var updateBody = new
        {
            programName = "Updated Program Name",
            sportId = TestIds.SportId,
            difficultyLevel = "Intermediate",
            minimumAge = 10,
            maximumAge = 18,
            durationWeeks = 16,
            capacity = 40
        };

        var response = await PutJsonAsync(client, $"{BaseRoute}/{programId}", updateBody);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);

        var program = content.Data!;
        Assert.Equal(programId, program.Id);
        Assert.Equal("Updated Program Name", program.ProgramName);
        Assert.Equal("Intermediate", program.DifficultyLevel);
        Assert.Equal(10, program.MinimumAge);
        Assert.Equal(18, program.MaximumAge);
        Assert.Equal(16, program.DurationWeeks);
        Assert.Equal(40, program.Capacity);
        Assert.NotNull(program.UpdatedAt);
    }

    [Fact]
    public async Task UpdateProgram_Returns404_WhenProgramNotFound()
    {
        var client = CreateClientAsRole("System Admin");
        var nonExistentId = Guid.NewGuid();

        var updateBody = new
        {
            programName = "Non Existent Program",
            sportId = TestIds.SportId,
            difficultyLevel = "Beginner",
            minimumAge = 8,
            maximumAge = 16,
            durationWeeks = 12,
            capacity = 30
        };

        var response = await PutJsonAsync(client, $"{BaseRoute}/{nonExistentId}", updateBody);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Delete Program

    [Fact]
    public async Task DeleteProgram_ReturnsOk_WhenSystemAdmin()
    {
        var client = CreateClientAsRole("System Admin");
        var programId = await CreateProgramViaApiAsync(client, "Program To Delete");

        var response = await DeleteAsync(client, $"{BaseRoute}/{programId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(programId, content.Data!.Id);

        var getResponse = await GetAsync(client, $"{BaseRoute}/{programId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProgram_Returns403_WhenAcademyAdmin()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramViaApiAsync(adminClient, "Program Academy Admin Cannot Delete");

        var academyAdminClient = CreateClientAsRole("Academy Admin");

        var response = await DeleteAsync(academyAdminClient, $"{BaseRoute}/{programId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Publish Program

    [Fact]
    public async Task PublishProgram_ReturnsOk_WhenDraftProgram()
    {
        var client = CreateClientAsRole("System Admin");
        var programId = await CreateProgramViaApiAsync(client, "Program To Publish");

        var programBeforePublish = await GetProgramFromApiAsync(client, programId);
        Assert.NotNull(programBeforePublish);

        var response = await PostAsync(client, $"{BaseRoute}/{programId}/publish");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(programId, content.Data!.Id);

        var programAfterPublish = await GetProgramFromApiAsync(client, programId);
        Assert.NotNull(programAfterPublish);
        Assert.NotEqual(programBeforePublish!.Status, programAfterPublish.Status);
    }

    #endregion

    #region Archive Program

    [Fact]
    public async Task ArchiveProgram_ReturnsOk_WhenActiveProgram()
    {
        var client = CreateClientAsRole("System Admin");
        var programId = await CreateProgramViaApiAsync(client, "Program To Archive");

        var publishResponse = await PostAsync(client, $"{BaseRoute}/{programId}/publish");
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);

        var programBeforeArchive = await GetProgramFromApiAsync(client, programId);
        Assert.NotNull(programBeforeArchive);

        var response = await PostAsync(client, $"{BaseRoute}/{programId}/archive");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal(programId, content.Data!.Id);

        var programAfterArchive = await GetProgramFromApiAsync(client, programId);
        Assert.NotNull(programAfterArchive);
        Assert.NotEqual(programBeforeArchive!.Status, programAfterArchive.Status);
    }

    #endregion

    #region Get Program By ID

    [Fact]
    public async Task GetProgram_ReturnsOk_WhenProgramExists()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var programId = await CreateProgramViaApiAsync(adminClient, "Program To Get");

        var anonymousClient = CreateAnonymousClient();
        var response = await GetAsync(anonymousClient, $"{BaseRoute}/{programId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);

        var program = content.Data!;
        Assert.Equal(programId, program.Id);
        Assert.Equal("Program To Get", program.ProgramName);
        Assert.Equal(TestIds.AcademyId, program.AcademyId);
        Assert.Equal(TestIds.SportId, program.SportId);
        Assert.Equal("Beginner", program.DifficultyLevel);
        Assert.Equal(8, program.MinimumAge);
        Assert.Equal(16, program.MaximumAge);
        Assert.Equal(12, program.DurationWeeks);
        Assert.Equal(30, program.Capacity);
        Assert.NotNull(program.ProgramCode);
    }

    [Fact]
    public async Task GetProgram_Returns404_WhenProgramNotFound()
    {
        var client = CreateAnonymousClient();
        var nonExistentId = Guid.NewGuid();

        var response = await GetAsync(client, $"{BaseRoute}/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Search Programs

    [Fact]
    public async Task SearchPrograms_ReturnsOk_WithResults()
    {
        var adminClient = CreateClientAsRole("System Admin");
        await CreateProgramViaApiAsync(adminClient, "Searchable Cricket Program");
        await CreateProgramViaApiAsync(adminClient, "Searchable Football Program");

        var anonymousClient = CreateAnonymousClient();
        var response = await GetAsync(anonymousClient, $"{BaseRoute}?academyId={TestIds.AcademyId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);

        var searchData = content.Data!;
        Assert.NotNull(searchData.Programs);
        Assert.NotEmpty(searchData.Programs);
        Assert.True(searchData.TotalCount >= 2);
        Assert.True(searchData.PageNumber >= 1);
        Assert.True(searchData.PageSize >= 1);
        Assert.True(searchData.TotalPages >= 1);

        Assert.Contains(searchData.Programs, p =>
            p.ProgramName == "Searchable Cricket Program" &&
            p.AcademyName == TestConstants.AcademyName);
        Assert.Contains(searchData.Programs, p =>
            p.ProgramName == "Searchable Football Program" &&
            p.AcademyName == TestConstants.AcademyName);
    }

    [Fact]
    public async Task SearchPrograms_ReturnsOk_WithPagination()
    {
        var adminClient = CreateClientAsRole("System Admin");
        await CreateProgramViaApiAsync(adminClient, "Pagination Program 1");
        await CreateProgramViaApiAsync(adminClient, "Pagination Program 2");
        await CreateProgramViaApiAsync(adminClient, "Pagination Program 3");

        var anonymousClient = CreateAnonymousClient();

        var page1Response = await GetAsync(anonymousClient, $"{BaseRoute}?academyId={TestIds.AcademyId}&page=1&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, page1Response.StatusCode);

        var page1Content = await page1Response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(page1Content);
        Assert.True(page1Content!.Success);
        Assert.NotNull(page1Content.Data);

        var page1Data = page1Content.Data!;
        Assert.Equal(2, page1Data.Programs.Count);
        Assert.Equal(1, page1Data.PageNumber);
        Assert.Equal(2, page1Data.PageSize);
        Assert.True(page1Data.TotalCount >= 3);
        Assert.True(page1Data.TotalPages >= 2);

        var page2Response = await GetAsync(anonymousClient, $"{BaseRoute}?academyId={TestIds.AcademyId}&page=2&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, page2Response.StatusCode);

        var page2Content = await page2Response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(page2Content);
        Assert.True(page2Content!.Success);
        Assert.NotNull(page2Content.Data);

        var page2Data = page2Content.Data!;
        Assert.Equal(2, page2Data.PageNumber);
        Assert.Equal(2, page2Data.PageSize);
        Assert.True(page2Data.Programs.Count >= 1);
    }

    #endregion
}