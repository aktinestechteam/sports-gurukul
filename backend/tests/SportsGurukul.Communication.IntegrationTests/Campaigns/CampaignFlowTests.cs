using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Controllers.V1;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.IntegrationTests.Campaigns;

public class CampaignFlowTests : CommunicationTestBase
{
    public CampaignFlowTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateCampaign_WithValidData_Returns201()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new CreateCampaignCommand(
            "Summer Campaign", "Campaign for summer offers",
            null, NotificationChannelType.Email,
            null, null, null);

        var response = await PostJsonAsync(client, "/api/v1/campaigns", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Name.Should().Be("Summer Campaign");
    }

    [Fact]
    public async Task GetCampaignById_WithExistingId_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateCampaignCommand(
            "Test Campaign", null,
            null, NotificationChannelType.Email,
            null, null, null);

        var createResponse = await PostJsonAsync(client, "/api/v1/campaigns", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();

        var getResponse = await GetAsync(client, $"/api/v1/campaigns/{created!.Data!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await getResponse.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();
        content!.Data!.Id.Should().Be(created.Data.Id);
    }

    [Fact]
    public async Task ScheduleCampaign_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateCampaignCommand(
            "Scheduled Campaign", null,
            null, NotificationChannelType.Email,
            null, null, null);

        var createResponse = await PostJsonAsync(client, "/api/v1/campaigns", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();
        var scheduleAt = DateTime.UtcNow.AddDays(1);

        var scheduleResponse = await PostJsonAsync(client,
            $"/api/v1/campaigns/{created!.Data!.Id}/schedule",
            new CampaignScheduleRequest(scheduleAt));

        scheduleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PauseCampaign_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateCampaignCommand(
            "Pause Test Campaign", null,
            null, NotificationChannelType.Email,
            null, null, null);

        var createResponse = await PostJsonAsync(client, "/api/v1/campaigns", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();

        var pauseResponse = await PostJsonAsync(client,
            $"/api/v1/campaigns/{created!.Data!.Id}/pause", new { });

        pauseResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResumeCampaign_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateCampaignCommand(
            "Resume Test Campaign", null,
            null, NotificationChannelType.Email,
            null, null, null);

        var createResponse = await PostJsonAsync(client, "/api/v1/campaigns", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();

        await PostJsonAsync(client, $"/api/v1/campaigns/{created!.Data!.Id}/pause", new { });

        var resumeResponse = await PostJsonAsync(client,
            $"/api/v1/campaigns/{created.Data.Id}/resume", new { });

        resumeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelCampaign_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateCampaignCommand(
            "Cancel Test Campaign", null,
            null, NotificationChannelType.Email,
            null, null, null);

        var createResponse = await PostJsonAsync(client, "/api/v1/campaigns", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();

        var cancelResponse = await PostJsonAsync(client,
            $"/api/v1/campaigns/{created!.Data!.Id}/cancel", new { });

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCampaign_WithoutAuth_Returns401()
    {
        var client = CreateAnonymousClient();
        var command = new CreateCampaignCommand(
            "Unauth Campaign", null,
            null, NotificationChannelType.Email,
            null, null, null);

        var response = await PostJsonAsync(client, "/api/v1/campaigns", command);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
