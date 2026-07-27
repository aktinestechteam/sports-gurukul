using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Tournament.IntegrationTests;

namespace Tournament.IntegrationTests;

public class TournamentAuthorizationTests : BaseIntegrationTest
{
    public TournamentAuthorizationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetTournament_AnonymousClient_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);

        var anonymousClient = CreateAnonymousClient();
        var response = await GetAsync(anonymousClient, $"api/v1/tournaments/{tournamentId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateTournament_AnonymousClient_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var command = new
        {
            TournamentName = "Unauthorized Tournament",
            TournamentType = TournamentType.League,
            SportId = Guid.NewGuid(),
            AcademyId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(60),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
            RegistrationType = RegistrationType.Individual
        };

        var response = await PostJsonAsync(client, "api/v1/tournaments", command);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTournament_Athlete_ReturnsForbidden()
    {
        var client = CreateClientAsRole("Athlete");

        var command = new
        {
            TournamentName = "Athlete Tournament",
            TournamentType = TournamentType.League,
            SportId = Guid.NewGuid(),
            AcademyId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(60),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
            RegistrationType = RegistrationType.Individual
        };

        var response = await PostJsonAsync(client, "api/v1/tournaments", command);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteTournament_Coach_ReturnsForbidden()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);

        var coachClient = CreateClientAsRole("Coach");
        var response = await DeleteAsync(coachClient, $"api/v1/tournaments/{tournamentId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchTournaments_AnonymousClient_ReturnsOk()
    {
        var client = CreateAnonymousClient();

        var response = await GetAsync(client, "api/v1/tournaments?searchTerm=Test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PublishTournament_AsCoach_ReturnsForbidden()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);

        var coachClient = CreateClientAsRole("Coach");
        var response = await PostAsync(coachClient, $"api/v1/tournaments/{tournamentId}/publish");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task OpenRegistration_AsAthlete_ReturnsForbidden()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var openCommand = new { RegistrationCloseDate = DateTime.UtcNow.AddDays(25) };
        var response = await PostJsonAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registration/open", openCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateTestTournamentAsync(HttpClient client)
    {
        var command = new
        {
            TournamentName = $"Auth Test Tournament {Guid.NewGuid():N}",
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
}