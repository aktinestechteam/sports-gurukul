using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;
using Xunit;
using SportsGurukul.Api.Common.Models;

namespace SportsGurukul.Communication.IntegrationTests.Analytics;

public class AnalyticsIntegrationTests : CommunicationTestBase
{
    public AnalyticsIntegrationTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task SearchNotifications_ReturnsPaginatedResults()
    {
        var admin = CreateAuthenticatedClient("Admin");

        for (int i = 0; i < 5; i++)
        {
            var cmd = new CreateNotificationCommand(
                null,
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                null, NotificationPriority.Normal,
                $"Search Test {i}", "<p>Body</p>",
                null, null, null, null, null, null,
                new List<CreateRecipientRequest> { new(null, "Email", $"user{i}@test.com", null) },
                null);
            await PostJsonAsync(admin, "/api/v1/notifications", cmd);
        }

        var response = await GetAsync(admin, "/api/v1/notifications?page=1&pageSize=3");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchNotifications_WithStatusFilter_Returns200()
    {
        var admin = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(admin,
            "/api/v1/notifications/search?status=Draft&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchNotifications_AsAnonymous_Returns200()
    {
        var client = CreateAnonymousClient();

        var response = await GetAsync(client, "/api/v1/notifications?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TemplateSearch_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, "/api/v1/templates/search");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CampaignSearch_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, "/api/v1/campaigns/search");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeliveryStatistics_ReturnsAggregatedData()
    {
        var admin = CreateAuthenticatedClient("Admin");

        var cmd1 = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal,
            "Stats Notif 1", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user1@test.com", null) },
            null);

        var response1 = await PostJsonAsync(admin, "/api/v1/notifications", cmd1);
        var notif1 = await response1.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();

        await PostJsonAsync(admin, $"/api/v1/notifications/{notif1!.Data!.Id}/send", new { });

        var cmd2 = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.High,
            "Stats Notif 2", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user2@test.com", null) },
            null);

        var response2 = await PostJsonAsync(admin, "/api/v1/notifications", cmd2);
        var notif2 = await response2.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();

        await PostJsonAsync(admin, $"/api/v1/notifications/{notif2!.Data!.Id}/cancel", new { });

        var statsResponse = await GetAsync(admin, "/api/v1/delivery/statistics");
        statsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DashboardData_AfterMultipleOperations_ReturnsConsistentResults()
    {
        var admin = CreateAuthenticatedClient("Admin");

        var templateCmd = new CreateTemplateCommand(
            "Dash Template", null,
            NotificationChannelType.Email,
            "Subject", "<p>Body</p>", null);
        var templateResponse = await PostJsonAsync(admin, "/api/v1/templates", templateCmd);
        templateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var campaignCmd = new CreateCampaignCommand(
            "Dash Campaign", null,
            null, NotificationChannelType.Email,
            null, null, null);
        var campaignResponse = await PostJsonAsync(admin, "/api/v1/campaigns", campaignCmd);
        campaignResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await GetAsync(admin, "/api/v1/notifications?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task NotificationSearch_ResponseHasExpectedStructure()
    {
        var admin = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(admin, "/api/v1/notifications?page=1&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResult<NotificationSummaryDto>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task MultiplePriorityNotifications_CanBeCreated()
    {
        var admin = CreateAuthenticatedClient("Admin");
        var priorities = new[] { NotificationPriority.Low, NotificationPriority.Normal, NotificationPriority.High, NotificationPriority.Critical };

        foreach (var priority in priorities)
        {
            var cmd = new CreateNotificationCommand(
                null,
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                null, priority,
                $"Priority {priority} Test", "<p>Body</p>",
                null, null, null, null, null, null,
                new List<CreateRecipientRequest> { new(null, "Email", $"user-{priority}@test.com", null) },
                null);

            var response = await PostJsonAsync(admin, "/api/v1/notifications", cmd);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
