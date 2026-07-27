using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Tournament.IntegrationTests;

namespace Tournament.IntegrationTests;

public class TournamentRegistrationTests : BaseIntegrationTest
{
    public TournamentRegistrationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterParticipant_AsAthlete_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreatePublishedTournamentWithRegistrationOpenAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");

        var command = new
        {
            CategoryId = (Guid?)null,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = TestIds.AthleteEntityId,
            TeamId = (Guid?)null,
            AcademyId = (Guid?)null,
            RegistrantName = TestConstants.AthleteName,
            Email = TestConstants.AthleteEmail,
            Phone = "+919200000004",
            Notes = "Registration test"
        };

        var response = await PostJsonAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registrations", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<ParticipantDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterParticipant_DuplicateRegistration_ReturnsConflict()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreatePublishedTournamentWithRegistrationOpenAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");

        var command = new
        {
            CategoryId = (Guid?)null,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = TestIds.AthleteEntityId,
            TeamId = (Guid?)null,
            AcademyId = (Guid?)null,
            RegistrantName = TestConstants.AthleteName,
            Email = TestConstants.AthleteEmail,
            Phone = "+919200000004",
            Notes = (string?)null
        };

        await PostJsonAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registrations", command);

        var response = await PostJsonAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registrations", command);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ApproveRegistration_AsSystemAdmin_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreatePublishedTournamentWithRegistrationOpenAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var registerCommand = new
        {
            CategoryId = (Guid?)null,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = TestIds.AthleteEntityId,
            TeamId = (Guid?)null,
            AcademyId = (Guid?)null,
            RegistrantName = TestConstants.AthleteName,
            Email = TestConstants.AthleteEmail,
            Phone = "+919200000004",
            Notes = (string?)null
        };
        await PostJsonAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registrations", registerCommand);

        var registrationId = await GetRegistrationIdAsync(adminClient, tournamentId);

        var response = await PostAsync(adminClient, $"api/v1/tournaments/{tournamentId}/registrations/{registrationId}/approve");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RejectRegistration_AsSystemAdmin_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreatePublishedTournamentWithRegistrationOpenAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var registerCommand = new
        {
            CategoryId = (Guid?)null,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = TestIds.AthleteEntityId,
            TeamId = (Guid?)null,
            AcademyId = (Guid?)null,
            RegistrantName = TestConstants.AthleteName,
            Email = TestConstants.AthleteEmail,
            Phone = "+919200000004",
            Notes = (string?)null
        };
        await PostJsonAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registrations", registerCommand);

        var registrationId = await GetRegistrationIdAsync(adminClient, tournamentId);

        var command = new { Reason = "Does not meet criteria" };
        var response = await PostJsonAsync(adminClient, $"api/v1/tournaments/{tournamentId}/registrations/{registrationId}/reject", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WithdrawRegistration_AsAthlete_ReturnsOk()
    {
        var adminClient = CreateClientAsRole("System Admin");
        var tournamentId = await CreatePublishedTournamentWithRegistrationOpenAsync(adminClient);

        var athleteClient = CreateClientAsRole("Athlete");
        var registerCommand = new
        {
            CategoryId = (Guid?)null,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = TestIds.AthleteEntityId,
            TeamId = (Guid?)null,
            AcademyId = (Guid?)null,
            RegistrantName = TestConstants.AthleteName,
            Email = TestConstants.AthleteEmail,
            Phone = "+919200000004",
            Notes = (string?)null
        };
        await PostJsonAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registrations", registerCommand);

        var registrationId = await GetRegistrationIdAsync(athleteClient, tournamentId);

        var response = await DeleteAsync(athleteClient, $"api/v1/tournaments/{tournamentId}/registrations/{registrationId}?reason=No longer interested");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreatePublishedTournamentWithRegistrationOpenAsync(HttpClient client)
    {
        var createCommand = new
        {
            TournamentName = $"Registration Test Tournament {Guid.NewGuid():N}",
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

        var createResponse = await PostJsonAsync(client, "api/v1/tournaments", createCommand);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TournamentDto>>();
        var tournamentId = createContent!.Data!.Id;

        await PostAsync(client, $"api/v1/tournaments/{tournamentId}/publish");

        var openCommand = new { RegistrationCloseDate = DateTime.UtcNow.AddDays(25) };
        await PostJsonAsync(client, $"api/v1/tournaments/{tournamentId}/registration/open", openCommand);

        return tournamentId;
    }

    private async Task<Guid> GetRegistrationIdAsync(HttpClient client, Guid tournamentId)
    {
        var response = await GetAsync(client, $"api/v1/tournaments/{tournamentId}/registrations");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<ParticipantDto>>>();
        return content!.Data!.First().Id;
    }
}