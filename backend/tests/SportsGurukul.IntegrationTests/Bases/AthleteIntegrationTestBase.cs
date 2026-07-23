using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Infrastructure;

namespace SportsGurukul.IntegrationTests.Bases;

[Collection("Postgres")]
public abstract class AthleteIntegrationTestBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient AdminClient;
    protected readonly HttpClient CoachClient;
    protected readonly HttpClient AthleteClient;
    protected readonly HttpClient UnauthenticatedClient;
    protected SeedResult SeedData = new();

    protected AthleteIntegrationTestBase(PostgresFixture postgresFixture)
    {
        Factory = new CustomWebApplicationFactory();
        Factory.SetConnectionString(postgresFixture.ConnectionString);

        AdminClient = Factory.CreateClient();
        CoachClient = Factory.CreateClient();
        AthleteClient = Factory.CreateClient();
        UnauthenticatedClient = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
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
            JwtTestHelper.GenerateToken(SeedData.AdminUserId, "admin@sportsgurukul.com", "Admin User", new[] { "Admin" }));
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

    protected async Task<AthleteDto?> CreateTestAthleteAsync(Guid? userId = null)
    {
        var targetUserId = userId ?? SeedData.AthleteUserId;
        var request = new
        {
            UserId = targetUserId,
            CurrentLevel = Domain.Enums.AthleteLevel.Intermediate,
            ExperienceYears = 5,
            Height = "5'10\"",
            Weight = "75kg",
            BloodGroup = Domain.Enums.BloodGroup.OPositive,
            DominantHand = Domain.Enums.DominantHand.Right,
            DominantFoot = Domain.Enums.DominantFoot.Right,
            Biography = "Test athlete biography"
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
            return content?.Data;
        }
        return null;
    }

    protected async Task<Domain.Entities.Athlete?> GetAthleteFromDbAsync(Guid athleteId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Athletes.FirstOrDefaultAsync(a => a.Id == athleteId);
    }

    protected async Task<int> GetTableCountAsync<T>(string tableName) where T : class
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Set<T>().CountAsync();
    }
}
