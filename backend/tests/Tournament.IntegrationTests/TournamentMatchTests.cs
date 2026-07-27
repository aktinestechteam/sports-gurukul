using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tournament.IntegrationTests;

namespace Tournament.IntegrationTests;

public class TournamentMatchTests : BaseIntegrationTest
{
    public TournamentMatchTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMatches_ForTournament_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        await SeedTestMatchAsync(tournamentId);

        var response = await GetAsync(adminClient, $"api/v1/tournaments/{tournamentId}/matches");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<MatchDto>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetMatches_WithStatusFilter_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        await SeedTestMatchAsync(tournamentId);

        var response = await GetAsync(adminClient, $"api/v1/tournaments/{tournamentId}/matches?status=Scheduled");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<MatchDto>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetMatch_ByValidId_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var response = await GetAsync(adminClient, $"api/v1/matches/{matchId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MatchDto>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Id.Should().Be(matchId);
    }

    [Fact]
    public async Task GetMatch_WithInvalidId_ReturnsNotFound()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var nonExistentId = Guid.NewGuid();

        var response = await GetAsync(adminClient, $"api/v1/matches/{nonExistentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task StartMatch_AsCoach_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var coachClient = CreateClientAsRole("Coach");
        var response = await PostAsync(coachClient, $"api/v1/matches/{matchId}/start");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StartMatch_AsAthlete_ReturnsForbidden()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var athleteClient = CreateClientAsRole("Athlete");
        var response = await PostAsync(athleteClient, $"api/v1/matches/{matchId}/start");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateScore_AsCoach_AfterStart_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var coachClient = CreateClientAsRole("Coach");
        await PostAsync(coachClient, $"api/v1/matches/{matchId}/start");

        var scoreCommand = new
        {
            HomeScore = 3,
            AwayScore = 2,
            ScoreDetails = "Set 1: 21-19, Set 2: 18-21, Set 3: 21-15"
        };

        var response = await PutJsonAsync(coachClient, $"api/v1/matches/{matchId}/score", scoreCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateScore_BeforeStart_ReturnsBadRequest()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var coachClient = CreateClientAsRole("Coach");
        var scoreCommand = new
        {
            HomeScore = 3,
            AwayScore = 2,
            ScoreDetails = (string?)null
        };

        var response = await PutJsonAsync(coachClient, $"api/v1/matches/{matchId}/score", scoreCommand);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteMatch_AsCoach_AfterScore_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var coachClient = CreateClientAsRole("Coach");
        await PostAsync(coachClient, $"api/v1/matches/{matchId}/start");

        var scoreCommand = new { HomeScore = 3, AwayScore = 2, ScoreDetails = (string?)null };
        await PutJsonAsync(coachClient, $"api/v1/matches/{matchId}/score", scoreCommand);

        var response = await PostAsync(coachClient, $"api/v1/matches/{matchId}/complete");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RecordWalkover_AsAcademyAdmin_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var academyClient = CreateClientAsRole("Academy Admin");
        await PostAsync(academyClient, $"api/v1/matches/{matchId}/start");

        var walkoverCommand = new
        {
            WinnerId = TestIds.AthleteEntityId,
            Notes = "Opponent did not show up"
        };

        var response = await PostJsonAsync(academyClient, $"api/v1/matches/{matchId}/walkover", walkoverCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RecordForfeit_AsAcademyAdmin_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);
        var matchId = await SeedTestMatchAsync(tournamentId);

        var academyClient = CreateClientAsRole("Academy Admin");
        await PostAsync(academyClient, $"api/v1/matches/{matchId}/start");

        var forfeitCommand = new
        {
            WinnerId = TestIds.AthleteEntityId,
            Notes = "Injury"
        };

        var response = await PostJsonAsync(academyClient, $"api/v1/matches/{matchId}/forfeit", forfeitCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateTestTournamentAsync(HttpClient client)
    {
        var command = new
        {
            TournamentName = $"Match Test Tournament {Guid.NewGuid():N}",
            TournamentType = TournamentType.League,
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(60),
            MaxParticipants = 16,
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
            RegistrationType = RegistrationType.Individual
        };

        var response = await PostJsonAsync(client, "api/v1/tournaments", command);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TournamentDto>>();
        return content!.Data!.Id;
    }

    private async Task<Guid> SeedTestMatchAsync(Guid tournamentId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var matchId = Guid.NewGuid();
        var participant1Id = Guid.NewGuid();
        var participant2Id = Guid.NewGuid();

        var participant1 = new TournamentParticipant
        {
            Id = participant1Id,
            TournamentId = tournamentId,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = TestIds.AthleteEntityId,
            ParticipantName = TestConstants.AthleteName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var participant2 = new TournamentParticipant
        {
            Id = participant2Id,
            TournamentId = tournamentId,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = TestIds.CoachEntityId,
            ParticipantName = TestConstants.CoachName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.TournamentParticipants.AddRange(participant1, participant2);

        var match = new TournamentMatch
        {
            Id = matchId,
            TournamentId = tournamentId,
            MatchNumber = 1,
            ScheduledDate = DateTime.UtcNow.AddDays(35),
            ScheduledTime = new TimeSpan(10, 0, 0),
            HomeParticipantId = participant1Id,
            AwayParticipantId = participant2Id,
            HomeParticipantName = TestConstants.AthleteName,
            AwayParticipantName = TestConstants.CoachName,
            Status = MatchStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.TournamentMatches.Add(match);
        await dbContext.SaveChangesAsync();

        return matchId;
    }
}