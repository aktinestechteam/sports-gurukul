using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests.Features.TrainingProgramManagement;

public class SearchApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    private const string BaseRoute = "api/v1/training-programs";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private bool _seeded;

    public SearchApiTests(CustomWebApplicationFactory factory) : base(factory) { }

    private async Task SeedProgramsAsync()
    {
        if (_seeded) return;

        var client = CreateClientAsRole("System Admin");

        var program1 = new
        {
            ProgramName = "Beginner Cricket Course",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "A beginner course for cricket",
            DifficultyLevel = 0,
            MinimumAge = 8,
            MaximumAge = 14,
            DurationWeeks = 12,
            Capacity = 25
        };
        var resp1 = await PostJsonAsync(client, BaseRoute, program1);
        resp1.EnsureSuccessStatusCode();

        var program2 = new
        {
            ProgramName = "Advanced Cricket Course",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "An advanced course for cricket",
            DifficultyLevel = 2,
            MinimumAge = 14,
            MaximumAge = 21,
            DurationWeeks = 24,
            Capacity = 20
        };
        var resp2 = await PostJsonAsync(client, BaseRoute, program2);
        resp2.EnsureSuccessStatusCode();
        var content2 = await resp2.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        var program2Id = content2!.Data!.Id;

        var publishResp = await PostAsync(client, $"{BaseRoute}/{program2Id}/publish");
        publishResp.EnsureSuccessStatusCode();

        var program3 = new
        {
            ProgramName = "Football Basics",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Introduction to football",
            DifficultyLevel = 0,
            MinimumAge = 6,
            MaximumAge = 12,
            DurationWeeks = 8,
            Capacity = 30
        };
        var resp3 = await PostJsonAsync(client, BaseRoute, program3);
        resp3.EnsureSuccessStatusCode();

        var program4 = new
        {
            ProgramName = "Cricket Masterclass",
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            Description = "Masterclass for advanced players",
            DifficultyLevel = 3,
            MinimumAge = 16,
            MaximumAge = 25,
            DurationWeeks = 16,
            Capacity = 15
        };
        var resp4 = await PostJsonAsync(client, BaseRoute, program4);
        resp4.EnsureSuccessStatusCode();
        var content4 = await resp4.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramDto>>(JsonOptions);
        var program4Id = content4!.Data!.Id;

        var archiveResp = await PostAsync(client, $"{BaseRoute}/{program4Id}/publish");
        archiveResp.EnsureSuccessStatusCode();
        archiveResp = await PostAsync(client, $"{BaseRoute}/{program4Id}/archive");
        archiveResp.EnsureSuccessStatusCode();

        _seeded = true;
    }

    #region Search Programs - Basic

    [Fact]
    public async Task SearchPrograms_ReturnsOk_WhenSearchTermMatches()
    {
        await SeedProgramsAsync();

        var client = CreateAnonymousClient();
        var response = await GetAsync(client, $"{BaseRoute}?searchTerm=Cricket");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);

        var searchData = content.Data!;
        Assert.True(searchData.TotalCount >= 2);
        Assert.All(searchData.Programs, p =>
            Assert.Contains("Cricket", p.ProgramName, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchPrograms_ReturnsEmpty_WhenNoMatch()
    {
        await SeedProgramsAsync();

        var client = CreateAnonymousClient();
        var response = await GetAsync(client, $"{BaseRoute}?searchTerm=SwimmingLessonsXYZ");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);

        var searchData = content.Data!;
        Assert.Equal(0, searchData.TotalCount);
        Assert.Empty(searchData.Programs);
    }

    [Fact]
    public async Task SearchPrograms_ReturnsPaginated_WhenMultiplePages()
    {
        await SeedProgramsAsync();

        var client = CreateAnonymousClient();

        var page1Response = await GetAsync(client, $"{BaseRoute}?page=1&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, page1Response.StatusCode);

        var page1Content = await page1Response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(page1Content);
        Assert.True(page1Content!.Success);
        Assert.NotNull(page1Content.Data);

        var page1Data = page1Content.Data!;
        Assert.Equal(2, page1Data.Programs.Count);
        Assert.Equal(1, page1Data.PageNumber);
        Assert.Equal(2, page1Data.PageSize);
        Assert.True(page1Data.TotalCount >= 4);
        Assert.True(page1Data.TotalPages >= 2);

        var page2Response = await GetAsync(client, $"{BaseRoute}?page=2&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, page2Response.StatusCode);

        var page2Content = await page2Response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(page2Content);
        Assert.True(page2Content!.Success);
        Assert.NotNull(page2Content.Data);

        var page2Data = page2Content.Data!;
        Assert.Equal(2, page2Data.PageNumber);
        Assert.Equal(2, page2Data.PageSize);
        Assert.True(page2Data.Programs.Count >= 2);

        var page1Ids = page1Data.Programs.Select(p => p.Id).ToHashSet();
        var page2Ids = page2Data.Programs.Select(p => p.Id).ToHashSet();
        Assert.Empty(page1Ids.Intersect(page2Ids));
    }

    #endregion

    #region Search Programs - Filters

    [Fact]
    public async Task SearchPrograms_FilterByAcademy_ReturnsCorrectResults()
    {
        await SeedProgramsAsync();

        var client = CreateAnonymousClient();
        var response = await GetAsync(client, $"{BaseRoute}?academyId={TestIds.AcademyId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);

        var searchData = content.Data!;
        Assert.True(searchData.TotalCount >= 4);
        Assert.All(searchData.Programs, p =>
            Assert.Equal(TestConstants.AcademyName, p.AcademyName));
    }

    [Fact]
    public async Task SearchPrograms_FilterByStatus_ReturnsCorrectResults()
    {
        await SeedProgramsAsync();

        var client = CreateAnonymousClient();

        var draftResponse = await GetAsync(client, $"{BaseRoute}?status=Draft");
        Assert.Equal(HttpStatusCode.OK, draftResponse.StatusCode);

        var draftContent = await draftResponse.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(draftContent);
        Assert.True(draftContent!.Success);
        Assert.NotNull(draftContent.Data);

        var draftData = draftContent.Data!;
        Assert.True(draftData.TotalCount >= 2);
        Assert.All(draftData.Programs, p =>
            Assert.Equal("Draft", p.Status));

        Assert.Contains(draftData.Programs, p => p.ProgramName == "Beginner Cricket Course");
        Assert.Contains(draftData.Programs, p => p.ProgramName == "Football Basics");

        var publishedResponse = await GetAsync(client, $"{BaseRoute}?status=Active");
        Assert.Equal(HttpStatusCode.OK, publishedResponse.StatusCode);

        var publishedContent = await publishedResponse.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(publishedContent);
        Assert.True(publishedContent!.Success);
        Assert.NotNull(publishedContent.Data);

        var publishedData = publishedContent.Data!;
        Assert.Contains(publishedData.Programs, p => p.ProgramName == "Advanced Cricket Course");

        var archivedResponse = await GetAsync(client, $"{BaseRoute}?status=Archived");
        Assert.Equal(HttpStatusCode.OK, archivedResponse.StatusCode);

        var archivedContent = await archivedResponse.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(archivedContent);
        Assert.True(archivedContent!.Success);
        Assert.NotNull(archivedContent.Data);

        var archivedData = archivedContent.Data!;
        Assert.Contains(archivedData.Programs, p => p.ProgramName == "Cricket Masterclass");
    }

    [Fact]
    public async Task SearchPrograms_FilterBySport_ReturnsCorrectResults()
    {
        await SeedProgramsAsync();

        var client = CreateAnonymousClient();
        var response = await GetAsync(client, $"{BaseRoute}?sportId={TestIds.SportId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);

        var searchData = content.Data!;
        Assert.True(searchData.TotalCount >= 4);
        Assert.All(searchData.Programs, p =>
            Assert.Equal(TestConstants.SportName, p.SportName));
    }

    #endregion

    #region Search Programs - Pagination

    [Fact]
    public async Task SearchPrograms_Pagination_ReturnsCorrectPage()
    {
        await SeedProgramsAsync();

        var client = CreateAnonymousClient();

        var pageResponse = await GetAsync(client, $"{BaseRoute}?page=1&pageSize=2&searchTerm=Cricket");
        Assert.Equal(HttpStatusCode.OK, pageResponse.StatusCode);

        var pageContent = await pageResponse.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(pageContent);
        Assert.True(pageContent!.Success);
        Assert.NotNull(pageContent.Data);

        var pageData = pageContent.Data!;
        Assert.Equal(1, pageData.PageNumber);
        Assert.Equal(2, pageData.PageSize);
        Assert.Equal(2, pageData.Programs.Count);
        Assert.True(pageData.TotalCount >= 2);
        Assert.True(pageData.TotalPages >= 1);

        var allIds = new HashSet<Guid>();
        for (int i = 1; i <= pageData.TotalPages; i++)
        {
            var pResponse = await GetAsync(client, $"{BaseRoute}?page={i}&pageSize=2&searchTerm=Cricket");
            Assert.Equal(HttpStatusCode.OK, pResponse.StatusCode);

            var pContent = await pResponse.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
            Assert.NotNull(pContent);
            Assert.True(pContent!.Success);

            foreach (var program in pContent.Data!.Programs)
            {
                Assert.True(allIds.Add(program.Id), $"Duplicate program ID found: {program.Id}");
            }
        }
    }

    #endregion

    #region Search Programs - No Auth Required

    [Fact]
    public async Task SearchPrograms_ReturnsOk_EvenWithoutAuth()
    {
        await SeedProgramsAsync();

        var anonClient = CreateAnonymousClient();
        var response = await GetAsync(anonClient, BaseRoute);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.True(content.Data.TotalCount >= 4);

        var searchResponse = await GetAsync(anonClient, $"{BaseRoute}/search?searchTerm=Football");
        Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);

        var searchContent = await searchResponse.Content.ReadFromJsonAsync<ApiResponse<TrainingProgramSearchResponse>>(JsonOptions);
        Assert.NotNull(searchContent);
        Assert.True(searchContent!.Success);
        Assert.Contains(searchContent.Data!.Programs, p => p.ProgramName == "Football Basics");
    }

    #endregion
}