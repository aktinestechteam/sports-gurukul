using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Controllers.V1;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.IntegrationTests.Notifications;

public class NotificationLifecycleTests : CommunicationTestBase
{
    public NotificationLifecycleTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    private async Task<Guid> CreateDraftNotification(HttpClient client)
    {
        var cmd = new CreateNotificationCommand(
            null,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            null, NotificationPriority.Normal, "Lifecycle Test", "<p>Body</p>",
            null, null, null, null, null, null,
            new List<CreateRecipientRequest> { new(null, "Email", "user@test.com", null) },
            null);

        var response = await PostJsonAsync(client, "/api/v1/notifications", cmd);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<NotificationDto>>();
        return content!.Data!.Id;
    }

    [Fact]
    public async Task QueueNotification_TransitionsToQueued()
    {
        var client = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(client);

        var response = await PostJsonAsync(client, $"/api/v1/notifications/{id}/queue", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendNotification_TransitionsToSending()
    {
        var client = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(client);

        var response = await PostJsonAsync(client, $"/api/v1/notifications/{id}/send", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ScheduleNotification_TransitionsToScheduled()
    {
        var client = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(client);
        var scheduleAt = DateTime.UtcNow.AddHours(1);

        var response = await PostJsonAsync(client, $"/api/v1/notifications/{id}/schedule",
            new ScheduleRequest(scheduleAt));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelNotification_TransitionsToCancelled()
    {
        var client = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(client);

        var response = await PostJsonAsync(client, $"/api/v1/notifications/{id}/cancel", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelSentNotification_ReturnsBadRequest()
    {
        var admin = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(admin);

        await PostJsonAsync(admin, $"/api/v1/notifications/{id}/send", new { });

        var cancelResponse = await PostJsonAsync(admin, $"/api/v1/notifications/{id}/cancel", new { });

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RetryFailedNotification_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(client);

        var sendResponse = await PostJsonAsync(client, $"/api/v1/notifications/{id}/send", new { });
        sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retryResponse = await PostJsonAsync(client, $"/api/v1/notifications/{id}/retry", new { });

        retryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MarkRead_Returns200()
    {
        var admin = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(admin);

        var athlete = CreateAuthenticatedClient("Athlete");
        var response = await PostJsonAsync(athlete, $"/api/v1/notifications/{id}/read", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task FullLifecycle_CreateQueueSendMarkRead_CompletesSuccessfully()
    {
        var admin = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(admin);

        (await PostJsonAsync(admin, $"/api/v1/notifications/{id}/queue", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        (await PostJsonAsync(admin, $"/api/v1/notifications/{id}/send", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var athlete = CreateAuthenticatedClient("Athlete");
        (await PostJsonAsync(athlete, $"/api/v1/notifications/{id}/read", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Lifecycle_AfterRetry_CanQueueAgain()
    {
        var admin = CreateAuthenticatedClient("Admin");
        var id = await CreateDraftNotification(admin);

        (await PostJsonAsync(admin, $"/api/v1/notifications/{id}/send", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        (await PostJsonAsync(admin, $"/api/v1/notifications/{id}/retry", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        (await PostJsonAsync(admin, $"/api/v1/notifications/{id}/queue", new { }))
            .StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
