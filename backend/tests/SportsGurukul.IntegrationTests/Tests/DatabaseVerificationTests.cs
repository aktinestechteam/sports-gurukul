using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Infrastructure;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class DatabaseVerificationTests : AthleteIntegrationTestBase
{
    public DatabaseVerificationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateAthlete_WritesToDatabase_Correctly()
    {
        var user = Builders.TestDataBuilder.CreateUser("DB Verify User", "dbverify@test.com");
        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        var response = await AdminClient.PostAsJsonAsync("/api/v1/athletes", new
        {
            UserId = user.Id,
            CurrentLevel = AthleteLevel.Advanced,
            ExperienceYears = 7,
            Height = "6'1\"",
            Weight = "82kg",
            BloodGroup = BloodGroup.APositive,
            DominantHand = DominantHand.Right,
            DominantFoot = DominantFoot.Ambidextrous,
            Biography = "DB verification athlete"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDto>>();
        var athleteId = content!.Data!.Id;

        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
            var dbAthlete = await dbContext.Athletes.FirstOrDefaultAsync(a => a.Id == athleteId);

            dbAthlete.Should().NotBeNull();
            dbAthlete!.UserId.Should().Be(user.Id);
            dbAthlete.CurrentLevel.Should().Be(AthleteLevel.Advanced);
            dbAthlete.ExperienceYears.Should().Be(7);
            dbAthlete.Height.Should().Be("6'1\"");
            dbAthlete.Weight.Should().Be("82kg");
            dbAthlete.BloodGroup.Should().Be(BloodGroup.APositive);
            dbAthlete.DominantHand.Should().Be(DominantHand.Right);
            dbAthlete.DominantFoot.Should().Be(DominantFoot.Ambidextrous);
            dbAthlete.Biography.Should().Be("DB verification athlete");
            dbAthlete.Status.Should().Be(AthleteStatus.Active);
            dbAthlete.IsDeleted.Should().BeFalse();
            dbAthlete.AthleteCode.Should().StartWith("ATH-");
            dbAthlete.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task UpdateAthlete_UpdatesDatabaseCorrectly()
    {
        var athlete = await CreateTestAthleteAsync();

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}", new
        {
            CurrentLevel = AthleteLevel.Expert,
            ExperienceYears = 15,
            Biography = "Updated for DB verification"
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbAthlete = await dbContext.Athletes.FirstOrDefaultAsync(a => a.Id == athlete.Id);

        dbAthlete.Should().NotBeNull();
        dbAthlete!.CurrentLevel.Should().Be(AthleteLevel.Expert);
        dbAthlete.ExperienceYears.Should().Be(15);
        dbAthlete.Biography.Should().Be("Updated for DB verification");
        dbAthlete.UpdatedAt.Should().NotBeNull();
        dbAthlete.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteAthlete_SoftDeletes_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();

        await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbAthlete = await dbContext.Athletes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == athlete.Id);

        dbAthlete.Should().NotBeNull();
        dbAthlete!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task RestoreAthlete_UnsoftDeletes_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}");

        await AdminClient.PostAsync($"/api/v1/athletes/{athlete.Id}/restore", null);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbAthlete = await dbContext.Athletes.FirstOrDefaultAsync(a => a.Id == athlete.Id);

        dbAthlete.Should().NotBeNull();
        dbAthlete!.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task AssignSport_CreatesJoinRecord_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();

        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbAthleteSport = await dbContext.AthleteSports
            .FirstOrDefaultAsync(s => s.AthleteId == athlete.Id && s.SportId == SeedData.CricketSportId);

        dbAthleteSport.Should().NotBeNull();
        dbAthleteSport!.IsPrimarySport.Should().BeTrue();
        dbAthleteSport.JoinedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task RemoveSport_DeletesJoinRecord_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/sports", new
        {
            SportId = SeedData.CricketSportId,
            IsPrimarySport = true
        });

        await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete.Id}/sports/{SeedData.CricketSportId}");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbAthleteSport = await dbContext.AthleteSports
            .FirstOrDefaultAsync(s => s.AthleteId == athlete.Id && s.SportId == SeedData.CricketSportId);

        dbAthleteSport.Should().BeNull();
    }

    [Fact]
    public async Task AddAchievement_CreatesRecords_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();

        await AdminClient.PostAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/achievements", new
        {
            Title = "DB Verify Achievement",
            Competition = "DB Verify Competition",
            Level = AchievementLevel.State,
            Date = DateTime.UtcNow.AddDays(-10)
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();

        var dbAchievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Title == "DB Verify Achievement");
        dbAchievement.Should().NotBeNull();
        dbAchievement!.Competition.Should().Be("DB Verify Competition");
        dbAchievement.Level.Should().Be(AchievementLevel.State);

        var dbAthleteAchievement = await dbContext.AthleteAchievements
            .FirstOrDefaultAsync(aa => aa.AthleteId == athlete.Id && aa.AchievementId == dbAchievement.Id);
        dbAthleteAchievement.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateMedicalProfile_CreatesRecord_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/medical-profile", new
        {
            MedicalConditions = "DB verify condition",
            BloodGroup = "AB+",
            InsuranceNumber = "DB-INS-001"
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbProfile = await dbContext.MedicalProfiles
            .FirstOrDefaultAsync(p => p.AthleteId == athlete.Id);

        dbProfile.Should().NotBeNull();
        dbProfile!.MedicalConditions.Should().Be("DB verify condition");
        dbProfile.BloodGroup.Should().Be("AB+");
        dbProfile.InsuranceNumber.Should().Be("DB-INS-001");
    }

    [Fact]
    public async Task UpdateEmergencyContact_CreatesRecord_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact", new
        {
            Name = "DB Verify Contact",
            Relationship = EmergencyRelationship.Coach,
            Phone = "+919999999999",
            Email = "dbverify@example.com"
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbContact = await dbContext.EmergencyContacts
            .FirstOrDefaultAsync(c => c.AthleteId == athlete.Id);

        dbContact.Should().NotBeNull();
        dbContact!.Name.Should().Be("DB Verify Contact");
        dbContact.Relationship.Should().Be(EmergencyRelationship.Coach);
        dbContact.Phone.Should().Be("+919999999999");
        dbContact.Email.Should().Be("dbverify@example.com");
    }

    [Fact]
    public async Task UpdateRanking_CreatesRecord_InDatabase()
    {
        var athlete = await CreateTestAthleteAsync();

        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/ranking", new
        {
            CurrentRank = "42",
            StateRank = "15",
            NationalRank = "500",
            InternationalRank = "2000",
            RankingAuthority = "DB Verify Authority"
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbRanking = await dbContext.Rankings
            .FirstOrDefaultAsync(r => r.AthleteId == athlete.Id);

        dbRanking.Should().NotBeNull();
        dbRanking!.CurrentRank.Should().Be("42");
        dbRanking.StateRank.Should().Be("15");
        dbRanking.NationalRank.Should().Be("500");
        dbRanking.InternationalRank.Should().Be("2000");
        dbRanking.RankingAuthority.Should().Be("DB Verify Authority");
    }
}
