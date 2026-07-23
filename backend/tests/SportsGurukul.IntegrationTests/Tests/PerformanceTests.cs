using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class PerformanceTests : AthleteIntegrationTestBase
{
    public PerformanceTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task GetAthletes_ListPage_CompletesWithinTimeLimit()
    {
        await CreateTestAthleteAsync();
        await CreateTestAthleteAsync();

        var sw = Stopwatch.StartNew();
        var response = await AdminClient.GetAsync("/api/v1/athletes?page=1&pageSize=20");
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "listing athletes should complete within 5 seconds");
    }

    [Fact]
    public async Task GetAthleteById_SingleRecord_CompletesWithinTimeLimit()
    {
        var athlete = await CreateTestAthleteAsync();

        var sw = Stopwatch.StartNew();
        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}");
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "getting a single athlete should complete within 3 seconds");
    }

    [Fact]
    public async Task SearchAthletes_BasicSearch_CompletesWithinTimeLimit()
    {
        await CreateTestAthleteAsync();

        var sw = Stopwatch.StartNew();
        var response = await AdminClient.GetAsync("/api/v1/athletes/search?searchTerm=Test&page=1&pageSize=10");
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "searching athletes should complete within 5 seconds");
    }

    [Fact]
    public async Task SearchAthletes_AdvancedFilters_CompletesWithinTimeLimit()
    {
        await CreateTestAthleteAsync();

        var sw = Stopwatch.StartNew();
        var response = await AdminClient.GetAsync(
            "/api/v1/athletes/search?currentLevel=Intermediate&minExperience=1&maxExperience=10&sortBy=name&page=1&pageSize=10");
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "advanced search should complete within 5 seconds");
    }

    [Fact]
    public async Task GetSuggestions_Autocomplete_CompletesWithinTimeLimit()
    {
        await CreateTestAthleteAsync();

        var sw = Stopwatch.StartNew();
        var response = await AdminClient.GetAsync("/api/v1/athletes/search/suggestions?prefix=Test&limit=10");
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "suggestions should complete within 3 seconds");
    }

    [Fact]
    public async Task CreateAndRetrieve_Athlete_CompletesWithinTimeLimit()
    {
        var user = Builders.TestDataBuilder.CreateUser("Perf User", "perf@test.com");
        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        var sw = Stopwatch.StartNew();

        var createResponse = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = user.Id,
            CurrentLevel = Domain.Enums.AthleteLevel.Intermediate,
            ExperienceYears = 3,
            Height = "5'9\"",
            Weight = "70kg"
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();

        var getResponse = await AdminClient.GetAsync($"/api/v1/athletes/{created!.Data!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        sw.Stop();
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "create + retrieve should complete within 5 seconds");
    }

    [Fact]
    public async Task FullCrudLifecycle_CompletesWithinTimeLimit()
    {
        var user = Builders.TestDataBuilder.CreateUser("Lifecycle User", "lifecycle@test.com");
        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        var sw = Stopwatch.StartNew();

        var createResponse = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = user.Id,
            CurrentLevel = Domain.Enums.AthleteLevel.Beginner,
            ExperienceYears = 1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
        var athleteId = created!.Data!.Id;

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athleteId}", new
        {
            CurrentLevel = Domain.Enums.AthleteLevel.Intermediate,
            ExperienceYears = 3
        });

        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athleteId}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });

        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athleteId}/achievements", new
        {
            Title = "Perf Achievement",
            Competition = "Perf Competition",
            Level = Domain.Enums.AchievementLevel.Local,
            Date = DateTime.UtcNow
        });

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athleteId}/medical-profile", new
        {
            BloodGroup = "O+"
        });

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athleteId}/emergency-contact", new
        {
            Name = "Perf Contact",
            Relationship = Domain.Enums.EmergencyRelationship.Parent,
            Phone = "+919876543210"
        });

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athleteId}/ranking", new
        {
            CurrentRank = "10"
        });

        var deleteResponse = await AdminClient.DeleteAsync($"/api/v1/athletes/{athleteId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var restoreResponse = await AdminClient.PostAsync($"/api/v1/athletes/{athleteId}/restore", null);
        restoreResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        sw.Stop();
        sw.ElapsedMilliseconds.Should().BeLessThan(15000,
            because: "full CRUD lifecycle should complete within 15 seconds");
    }
}

