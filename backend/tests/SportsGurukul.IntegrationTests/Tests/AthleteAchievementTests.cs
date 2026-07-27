using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class AthleteAchievementTests : AthleteIntegrationTestBase
{
    public AthleteAchievementTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task AddAchievement_ValidRequest_AddsSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "State Championship Winner",
            Competition = "State Cricket Championship 2024",
            Position = "1st",
            Level = AchievementLevel.State,
            Date = DateTime.UtcNow.AddDays(-30),
            Notes = "Won the final match"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteAchievementDto>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Title.Should().Be("State Championship Winner");
        content.Data.Competition.Should().Be("State Cricket Championship 2024");
        content.Data.Position.Should().Be("1st");
        content.Data.Level.Should().Be("State");
        content.Data.Notes.Should().Be("Won the final match");
    }

    [Fact]
    public async Task AddAchievement_NonExistentAthlete_ReturnsNotFound()
    {
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{Guid.NewGuid()}/achievements", new
        {
            Title = "Test Achievement",
            Competition = "Test Competition",
            Position = "1st",
            Level = AchievementLevel.Local,
            Date = DateTime.UtcNow
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddMultipleAchievements_ReturnsAll()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "Achievement 1",
            Competition = "Competition 1",
            Level = AchievementLevel.Local,
            Date = DateTime.UtcNow.AddDays(-60)
        });
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete.Id}/achievements", new
        {
            Title = "Achievement 2",
            Competition = "Competition 2",
            Level = AchievementLevel.State,
            Date = DateTime.UtcNow.AddDays(-30)
        });

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/achievements");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteAchievementDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAchievements_NoAchievements_ReturnsEmptyList()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/achievements");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteAchievementDto>>>();
        content!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAchievement_ValidRequest_UpdatesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        var addResponse = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "Original Title",
            Competition = "Original Competition",
            Level = AchievementLevel.Local,
            Date = DateTime.UtcNow.AddDays(-30)
        });
        var addedContent = await addResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteAchievementDto>>();
        var achievementId = addedContent!.Data!.Id;

        var response = await AdminClient.PutAsJsonAsync(
            $"/api/v1/athletes/{athlete.Id}/achievements/{achievementId}", new
        {
            Title = "Updated Title",
            Competition = "Updated Competition",
            Level = AchievementLevel.National
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteAchievementDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Title.Should().Be("Updated Title");
        content.Data.Competition.Should().Be("Updated Competition");
        content.Data.Level.Should().Be("National");
    }

    [Fact]
    public async Task UpdateAchievement_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync(
            $"/api/v1/athletes/{athlete!.Id}/achievements/{Guid.NewGuid()}", new
        {
            Title = "Updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAchievement_Exists_DeletesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        var addResponse = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "To Delete",
            Competition = "Competition",
            Level = AchievementLevel.Local,
            Date = DateTime.UtcNow
        });
        var addedContent = await addResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteAchievementDto>>();
        var achievementId = addedContent!.Data!.Id;

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete.Id}/achievements/{achievementId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/achievements");
        var getContent = await getResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteAchievementDto>>>();
        getContent!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAchievement_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}/achievements/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Achievements_FullLifecycle_WorksCorrectly()
    {
        var athlete = await CreateTestAthleteAsync();

        var addResponse = await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "Lifecycle Achievement",
            Competition = "Test Competition",
            Level = AchievementLevel.District,
            Date = DateTime.UtcNow.AddDays(-10)
        });
        addResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var added = await addResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteAchievementDto>>();

        var getResponse = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/achievements");
        var getContent = await getResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteAchievementDto>>>();
        getContent!.Data.Should().HaveCount(1);

        var updateResponse = await AdminClient.PutAsJsonAsync(
            $"/api/v1/athletes/{athlete.Id}/achievements/{added!.Data!.Id}", new
        {
            Title = "Updated Lifecycle",
            Level = AchievementLevel.State
        });
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await AdminClient.DeleteAsync(
            $"/api/v1/athletes/{athlete.Id}/achievements/{added.Data.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var finalResponse = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/achievements");
        var finalContent = await finalResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteAchievementDto>>>();
        finalContent!.Data.Should().BeEmpty();
    }
}
