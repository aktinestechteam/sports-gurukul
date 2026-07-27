using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Tournament.IntegrationTests;

namespace Tournament.IntegrationTests;

public class TournamentCrudTests : BaseIntegrationTest
{
    public TournamentCrudTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateTournament_AsSystemAdmin_ReturnsOk()
    {
        var client = CreateClientAsRole("System Admin");

        var command = new
        {
            TournamentName = "Test Tournament 2026",
            Description = "A test tournament",
            TournamentType = TournamentType.League,
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(60),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
            MaxParticipants = 16,
            MinParticipants = 4,
            RegistrationType = RegistrationType.Individual
        };

        var response = await PostJsonAsync(client, "api/v1/tournaments", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TournamentDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.TournamentName.Should().Be("Test Tournament 2026");
        content.Data.TournamentType.Should().Be(TournamentType.League);
    }

    [Fact]
    public async Task CreateTournament_AsCoach_ReturnsForbidden()
    {
        var client = CreateClientAsRole("Coach");

        var command = new
        {
            TournamentName = "Test Tournament 2026",
            TournamentType = TournamentType.League,
            SportId = TestIds.SportId,
            AcademyId = TestIds.AcademyId,
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
    public async Task GetTournament_ExistingId_ReturnsOk()
    {
        var client = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(client);

        var response = await GetAsync(client, $"api/v1/tournaments/{tournamentId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TournamentDto>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Id.Should().Be(tournamentId);
    }

    [Fact]
    public async Task GetTournament_NonExistentId_ReturnsNotFound()
    {
        var client = CreateClientAsRole("System Admin");
        var nonExistentId = Guid.NewGuid();

        var response = await GetAsync(client, $"api/v1/tournaments/{nonExistentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchTournaments_ReturnsOk()
    {
        var client = CreateClientAsRole("System Admin");
        await CreateTestTournamentAsync(client);

        var response = await GetAsync(client, "api/v1/tournaments?searchTerm=Test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<TournamentSummaryDto>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DeleteTournament_AsSystemAdmin_ReturnsOk()
    {
        var client = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(client);

        var response = await DeleteAsync(client, $"api/v1/tournaments/{tournamentId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTournament_AsAcademyAdmin_ReturnsForbidden()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(adminClient);

        var academyClient = CreateClientAsRole("Academy Admin");
        var response = await DeleteAsync(academyClient, $"api/v1/tournaments/{tournamentId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PublishTournament_AsSystemAdmin_ReturnsOk()
    {
        var client = CreateClientAsRole("System Admin");
        var tournamentId = await CreateTestTournamentAsync(client);

        var response = await PostAsync(client, $"api/v1/tournaments/{tournamentId}/publish");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateTestTournamentAsync(HttpClient client)
    {
        var command = new
        {
            TournamentName = $"Tournament {Guid.NewGuid():N}",
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