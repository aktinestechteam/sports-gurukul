using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.IntegrationTests.Infrastructure;
using Xunit;

namespace SportsGurukul.IntegrationTests.Bases;

[Collection("Postgres")]
public abstract class AcademyIntegrationTestBase : IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient AdminClient;
    protected readonly HttpClient CoachClient;
    protected readonly HttpClient AthleteClient;
    protected readonly HttpClient AcademyAdminClient;
    protected readonly HttpClient UnauthenticatedClient;
    protected SeedResult SeedData = new();

    protected AcademyIntegrationTestBase(PostgresFixture postgresFixture)
    {
        Factory = new TestWebApplicationFactory();
        Factory.SetConnectionString(postgresFixture.ConnectionString);

        AdminClient = Factory.CreateClient();
        CoachClient = Factory.CreateClient();
        AthleteClient = Factory.CreateClient();
        AcademyAdminClient = Factory.CreateClient();
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
        AcademyAdminClient.Dispose();
        UnauthenticatedClient.Dispose();
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
        AcademyAdminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(SeedData.AcademyAdminUserId, "academy@sportsgurukul.com", "Academy Admin User", new[] { "Academy Admin" }));
    }

    protected HttpClient CreateClientWithRole(string role, Guid userId, string email, string fullName)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            JwtTestHelper.GenerateToken(userId, email, fullName, new[] { role }));
        return client;
    }

    protected async Task<Academy?> GetAcademyFromDbAsync(Guid academyId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Academies.FirstOrDefaultAsync(a => a.Id == academyId);
    }

    protected async Task<Academy?> GetDeletedAcademyFromDbAsync(Guid academyId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Academies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == academyId && a.IsDeleted);
    }

    protected async Task<int> GetAcademyCountAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Academies.CountAsync();
    }

    protected async Task<Guid> CreateAcademyDirectlyInDbAsync(string? name = null, string? email = null)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var academy = new Domain.Entities.Academy
        {
            Id = Guid.NewGuid(),
            AcademyCode = $"ACAD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            Name = name ?? $"Test Academy {Guid.NewGuid().ToString()[..6]}",
            Email = email ?? $"academy{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = $"+919{Random.Shared.Next(100000000, 999999999)}",
            Status = AcademyStatus.Active,
            VerificationStatus = VerificationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Academies.Add(academy);
        await dbContext.SaveChangesAsync();
        return academy.Id;
    }

    protected async Task<Guid> CreateVerifiedAcademyDirectlyInDbAsync(string? name = null)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var academy = new Domain.Entities.Academy
        {
            Id = Guid.NewGuid(),
            AcademyCode = $"ACAD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[4].ToString().ToUpper()}{Guid.NewGuid().ToString()[5].ToString().ToUpper()}{Guid.NewGuid().ToString()[6].ToString().ToUpper()}{Guid.NewGuid().ToString()[7].ToString().ToUpper()}",
            Name = name ?? $"Verified Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"verified{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = $"+919{Random.Shared.Next(100000000, 999999999)}",
            Status = AcademyStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Academies.Add(academy);
        await dbContext.SaveChangesAsync();
        return academy.Id;
    }

    protected async Task<Coach?> CreateCoachDirectlyInDbAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var coachUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = $"Coach {Guid.NewGuid().ToString()[..6]}",
            Email = $"coach{Guid.NewGuid().ToString()[..6]}@test.com",
            PhoneNumber = $"+919{Random.Shared.Next(100000000, 999999999)}",
            PasswordHash = "hashed",
            Status = UserStatus.Active,
            IsEmailVerified = true
        };

        var coach = new Coach
        {
            Id = Guid.NewGuid(),
            UserId = coachUser.Id,
            CoachCode = $"COA-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            Biography = "Test coach",
            Status = CoachStatus.Active,
            VerificationStatus = VerificationStatus.Verified
        };

        dbContext.Users.Add(coachUser);
        dbContext.Coaches.Add(coach);
        await dbContext.SaveChangesAsync();
        return coach;
    }

    protected async Task<Athlete?> CreateAthleteDirectlyInDbAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var athleteUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = $"Athlete {Guid.NewGuid().ToString()[..6]}",
            Email = $"athlete{Guid.NewGuid().ToString()[..6]}@test.com",
            PhoneNumber = $"+919{Random.Shared.Next(100000000, 999999999)}",
            PasswordHash = "hashed",
            Status = UserStatus.Active,
            IsEmailVerified = true
        };

        var athlete = new Domain.Entities.Athlete
        {
            Id = Guid.NewGuid(),
            UserId = athleteUser.Id,
            AthleteCode = $"ATH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CurrentLevel = AthleteLevel.Intermediate,
            ExperienceYears = 3,
            Status = AthleteStatus.Active,
            RegistrationDate = DateTime.UtcNow
        };

        dbContext.Users.Add(athleteUser);
        dbContext.Athletes.Add(athlete);
        await dbContext.SaveChangesAsync();
        return athlete;
    }

    protected async Task<Guid> CreateAcademyViaApiAsync(string? name = null, string? email = null)
    {
        var request = new
        {
            Name = name ?? $"Integration Academy {Guid.NewGuid().ToString()[..6]}",
            Email = email ?? $"intacademy{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = $"+919{Random.Shared.Next(100000000, 999999999)}",
            Description = "Integration test academy"
        };

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
            return content?.Data?.Id ?? Guid.Empty;
        }
        return Guid.Empty;
    }
}