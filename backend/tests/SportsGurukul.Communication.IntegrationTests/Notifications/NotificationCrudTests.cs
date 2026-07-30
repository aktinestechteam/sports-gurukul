using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.IntegrationTests.Notifications;

public class NotificationCrudTests : CommunicationTestBase
{
    public NotificationCrudTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateNotification_WithValidData_Returns201()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new CreateNotificationCommand(
            TemplateId: null,
            ChannelId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test Notification",
            Body: "<p>Hello from integration test</p>",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(null, "Email", "user@test.com", "Test User")
            },
            Attachments: null
        );

        var response = await PostJsonAsync(client, "/api/v1/notifications", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Subject.Should().Be("Test Notification");
        content.Data.Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public async Task CreateNotification_WithoutAuth_Returns401()
    {
        var client = CreateAnonymousClient();
        var command = new CreateNotificationCommand(
            TemplateId: null,
            ChannelId: Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal, "Test", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user@test.com", null) },
            null);

        var response = await PostJsonAsync(client, "/api/v1/notifications", command);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetNotificationById_WithExistingId_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal, "Test Get", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user@test.com", null) },
            null);

        var createResponse = await PostJsonAsync(client, "/api/v1/notifications", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();

        var getResponse = await GetAsync(client, $"/api/v1/notifications/{created!.Data!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await getResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();
        content!.Data!.Id.Should().Be(created.Data.Id);
    }

    [Fact]
    public async Task GetNotificationById_WithNonExistentId_Returns404()
    {
        var client = CreateAuthenticatedClient("Admin");
        var id = Guid.NewGuid();

        var response = await GetAsync(client, $"/api/v1/notifications/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateNotification_WithValidData_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal, "Original", "<p>Original</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user@test.com", null) },
            null);

        var createResponse = await PostJsonAsync(client, "/api/v1/notifications", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();

        var updateCmd = new UpdateNotificationCommand(
            created!.Data!.Id, "Updated Subject", null, null, null, null, null);

        var updateResponse = await PutJsonAsync(client, $"/api/v1/notifications/{created.Data.Id}", updateCmd);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();
        content!.Data!.Subject.Should().Be("Updated Subject");
    }

    [Fact]
    public async Task DeleteNotification_WithExistingId_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal, "To Delete", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user@test.com", null) },
            null);

        var createResponse = await PostJsonAsync(client, "/api/v1/notifications", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();

        var deleteResponse = await DeleteAsync(client, $"/api/v1/notifications/{created!.Data!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteNotification_WithoutPermission_Returns403()
    {
        var client = CreateAuthenticatedClient("Academy Admin");
        var cmd = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal, "Test", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user@test.com", null) },
            null);

        var createResponse = await PostJsonAsync(client, "/api/v1/notifications", cmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();

        var deleteResponse = await DeleteAsync(client, $"/api/v1/notifications/{created!.Data!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SearchNotifications_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, "/api/v1/notifications?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
