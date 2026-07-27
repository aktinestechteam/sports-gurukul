using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Infrastructure;
using Xunit;

namespace SportsGurukul.IntegrationTests.Bases;

[Collection("Postgres")]
public abstract class CoachIntegrationTestBase : IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient AdminClient;
    protected readonly HttpClient CoachClient;
    protected readonly HttpClient AthleteClient;
    protected readonly HttpClient UnauthenticatedClient;
    protected SeedResult SeedData = new();

    protected CoachIntegrationTestBase(PostgresFixture postgresFixture)
    {
        Factory = new TestWebApplicationFactory();
        Factory.SetConnectionString(postgresFixture.ConnectionString);

        AdminClient = Factory.CreateClient();
        CoachClient = Factory.CreateClient();
        AthleteClient = Factory.CreateClient();
        UnauthenticatedClient = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Factory.InitializeAsync();
        await Factory.ResetDatabaseAsync();
        await SeedDatabaseAsync();
        SetAuthHeaders();
    }

    public async Task DisposeAsync()
    {
        AdminClient.Dispose();
        CoachClient.Dispose();
        AthleteClient.Dispose();
        UnauthenticatedClient.Dispose();
        await Factory.DisposeAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seeder = new DatabaseSeeder(dbContext, passwordHasher);
        SeedData = await seeder.SeedAsync();
    }

    private void SetAuthHeaders()
    {
        AdminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(SeedData.AdminUserId, "admin@sportsgurukul.com", "Admin User", new[] { "System Admin" }));
        CoachClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(SeedData.CoachUserId, "coach@sportsgurukul.com", "Coach User", new[] { "Coach" }));
        AthleteClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(SeedData.AthleteUserId, "athlete@sportsgurukul.com", "Athlete User", new[] { "Athlete" }));
    }

    protected HttpClient CreateClientWithRole(string role, Guid userId, string email, string fullName)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(userId, email, fullName, new[] { role }));
        return client;
    }

    protected async Task<CoachDto?> CreateTestCoachAsync(Guid? userId = null)
    {
        var targetUserId = userId ?? SeedData.CoachUserId;
        var request = new CreateCoachRequest
        {
            UserId = targetUserId,
            Biography = "Test coach biography",
            YearsOfExperience = 8,
            CurrentOrganization = "Test Sports Academy",
            HighestQualification = "BCCI Level A",
            PreferredLanguage = "English",
            CoachingLevel = CoachingLevel.Senior
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/coach", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDto>>();
            return content?.Data;
        }
        return null;
    }

    protected async Task<CoachProfileDto?> GetCoachProfileAsync(Guid coachId)
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coachId}");
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachProfileDto>>();
            return content?.Data;
        }
        return null;
    }

    protected async Task<Domain.Entities.Coach?> GetCoachFromDbAsync(Guid coachId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Coaches.FirstOrDefaultAsync(c => c.Id == coachId);
    }

    protected async Task<int> GetTableCountAsync<T>() where T : class
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Set<T>().CountAsync();
    }

    protected async Task<Domain.Entities.Coach?> GetCoachByUserIdFromDbAsync(Guid userId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Coaches.FirstOrDefaultAsync(c => c.UserId == userId);
    }
}