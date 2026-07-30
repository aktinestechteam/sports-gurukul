using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;
using Xunit;
using SportsGurukul.Api.Common.Models;

namespace SportsGurukul.Communication.IntegrationTests.CrossModule;

public class EndToEndNotificationWorkflowTests : CommunicationTestBase
{
    public EndToEndNotificationWorkflowTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task FullWorkflow_CreateTemplate_ThenNotification_WithCampaign()
    {
        var admin = CreateAuthenticatedClient("Admin");

        var templateCmd = new CreateTemplateCommand(
            "Workflow Template", "Integration test template",
            NotificationChannelType.Email,
            "Hello {{name}}!", "<h1>Hello {{name}}</h1>",
            new List<CreateTemplateVariableRequest>
            {
                new("name", "User name", true, null, "string")
            });

        var templateResponse = await PostJsonAsync(admin, "/api/v1/templates", templateCmd);
        templateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var template = await templateResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();

        await PostJsonAsync(admin, $"/api/v1/templates/{template!.Data!.Id}/publish", new { });

        var campaignCmd = new CreateCampaignCommand(
            "Workflow Campaign", "Campaign for workflow test",
            template.Data.Id, NotificationChannelType.Email,
            DateTime.UtcNow.AddDays(7), "all-users", null);

        var campaignResponse = await PostJsonAsync(admin, "/api/v1/campaigns", campaignCmd);
        campaignResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var campaign = await campaignResponse.Content.ReadFromJsonAsync<ApiResponse<CampaignDto>>();

        var notifCmd = new CreateNotificationCommand(
            template.Data.Id,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.High,
            "Hello User!", "<h1>Hello User</h1>",
            null, null, null, campaign!.Data!.Id, null, null,
            new List<CreateRecipientRequest>
            {
                new(Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    "Email", "user@test.com", "Test User")
            },
            null);

        var notifResponse = await PostJsonAsync(admin, "/api/v1/notifications", notifCmd);
        notifResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var notification = await notifResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();

        (await PostJsonAsync(admin, $"/api/v1/notifications/{notification!.Data!.Id}/queue", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        (await PostJsonAsync(admin, $"/api/v1/notifications/{notification.Data.Id}/send", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var athlete = CreateAuthenticatedClient("Athlete");
        (await PostJsonAsync(athlete, $"/api/v1/notifications/{notification.Data.Id}/read", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await GetAsync(admin, $"/api/v1/notifications/{notification.Data.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CrossModule_PreferenceCheck_BeforeNotification()
    {
        var admin = CreateAuthenticatedClient("Admin");

        var muteCmd = new MuteChannelCommand(
            Guid.Parse("30000000-0000-0000-0000-000000000001"), NotificationChannelType.Email);
        var muteResponse = await PostJsonAsync(admin, "/api/v1/preferences/mute", muteCmd);
        muteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var notifCmd = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal,
            "Preference Check", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest>
            {
                new(Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    "Email", "user@test.com", "Test User")
            },
            null);

        var notifResponse = await PostJsonAsync(admin, "/api/v1/notifications", notifCmd);
        notifResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var unmuteCmd = new UnmuteChannelCommand(
            Guid.Parse("30000000-0000-0000-0000-000000000001"), NotificationChannelType.Email);
        var unmuteResponse = await PostJsonAsync(admin, "/api/v1/preferences/unmute", unmuteCmd);
        unmuteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CrossModule_TemplateRendering_ThenNotification()
    {
        var admin = CreateAuthenticatedClient("Admin");

        var templateCmd = new CreateTemplateCommand(
            "Rendering Template", null,
            NotificationChannelType.Email,
            "Welcome {{name}} to {{product}}!",
            "<p>Hi {{name}}, welcome to {{product}}!</p>",
            new List<CreateTemplateVariableRequest>
            {
                new("name", null, true, null, "string"),
                new("product", null, true, "SportsGurukul", "string")
            });

        var templateResponse = await PostJsonAsync(admin, "/api/v1/templates", templateCmd);
        var template = await templateResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();

        var notifCmd = new CreateNotificationCommand(
            template!.Data!.Id,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal,
            "Welcome Test to SportsGurukul!", "<p>Hi Test, welcome!</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest>
            {
                new(null, "Email", "test@example.com", "Test")
            },
            null);

        var notifResponse = await PostJsonAsync(admin, "/api/v1/notifications", notifCmd);
        notifResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Workflow_WithBusinessRuleFailure_Returns400()
    {
        RuleValidator.ShouldFail = true;
        RuleValidator.FailureMessage = "Rate limit exceeded";

        var admin = CreateAuthenticatedClient("Admin");

        var command = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal,
            "Rate Limited", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user@test.com", null) },
            null);

        var response = await PostJsonAsync(admin, "/api/v1/notifications", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        RuleValidator.Reset();
    }

    [Fact]
    public async Task Analytics_GetStatistics_AfterCreatingNotifications()
    {
        var admin = CreateAuthenticatedClient("Admin");

        for (int i = 0; i < 3; i++)
        {
            var cmd = new CreateNotificationCommand(
                null,
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                null, NotificationPriority.Normal,
                $"Analytics Test {i}", "<p>Body</p>",
                null, null, null, null, null, null,
                new List<CreateRecipientRequest> { new(null, "Email", $"user{i}@test.com", null) },
                null);

            var response = await PostJsonAsync(admin, "/api/v1/notifications", cmd);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        var statsResponse = await GetAsync(admin, "/api/v1/delivery/statistics");
        statsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
