using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class CoachValidationTests : CoachIntegrationTestBase
{
    public CoachValidationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateCoach_EmptyUserId_ReturnsBadRequest()
    {
        var request = new CreateCoachRequest
        {
            UserId = Guid.Empty,
            Biography = "Valid bio",
            YearsOfExperience = 5
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCoach_NegativeExperience_ReturnsBadRequest()
    {
        var user = Builders.TestDataBuilder.CreateUser("Neg Exp", "negexp@test.com");
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var request = new CreateCoachRequest
        {
            UserId = user.Id,
            Biography = "Valid bio",
            YearsOfExperience = -5
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCertification_EmptyName_ReturnsBadRequest()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new AddCertificationRequest
        {
            CertificationName = "",
            IssuingAuthority = "Org",
            IssueDate = DateTime.UtcNow
        };

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCertification_EmptyIssuingAuthority_ReturnsBadRequest()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new AddCertificationRequest
        {
            CertificationName = "Cert Name",
            IssuingAuthority = "",
            IssueDate = DateTime.UtcNow
        };

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AssignSport_EmptySportId_ReturnsBadRequest()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new CoachAssignSportRequest { SportId = Guid.Empty };

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCoach_VerifiesDbState()
    {
        var user = Builders.TestDataBuilder.CreateUser("DB Test Coach", "dbcoach@test.com");
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var request = new CreateCoachRequest
        {
            UserId = user.Id,
            Biography = "DB verification coach",
            YearsOfExperience = 8,
            CoachingLevel = SportsGurukul.Domain.Enums.CoachingLevel.Intermediate
        };

        var response = await AdminClient.PostAsJsonAsync("/api/v1/coach", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var verifyScope = Factory.Services.CreateScope();
        var verifyDbContext = verifyScope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var coach = verifyDbContext.Coaches.FirstOrDefault(c => c.UserId == user.Id);
        coach.Should().NotBeNull();
        coach!.Biography.Should().Be("DB verification coach");
        coach.YearsOfExperience.Should().Be(8);
        coach.CoachCode.Should().StartWith("COACH-");
    }

    [Fact]
    public async Task AssignSport_VerifiesDbState()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", new CoachAssignSportRequest
        {
            SportId = SeedData.CricketSportId
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var coachSport = dbContext.CoachSports.FirstOrDefault(cs => cs.CoachId == coach.Id);
        coachSport.Should().NotBeNull();
    }

    [Fact]
    public async Task AddCertification_VerifiesDbState()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", new AddCertificationRequest
        {
            CertificationName = "DB Cert",
            IssuingAuthority = "DB Org",
            IssueDate = new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var cert = dbContext.CoachCertifications.FirstOrDefault(c => c.CoachId == coach.Id);
        cert.Should().NotBeNull();
        cert!.CertificationName.Should().Be("DB Cert");
        cert.IssuingAuthority.Should().Be("DB Org");
    }

    [Fact]
    public async Task DeleteCoach_VerifiesDbState()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SportsGurukul.Infrastructure.Persistence.ApplicationDbContext>();
        var dbCoach = dbContext.Coaches.IgnoreQueryFilters().FirstOrDefault(c => c.Id == coach.Id);
        dbCoach.Should().NotBeNull();
        dbCoach!.IsDeleted.Should().BeTrue();
    }
}
