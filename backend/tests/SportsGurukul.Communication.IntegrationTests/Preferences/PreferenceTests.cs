using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.IntegrationTests.Preferences;

public class PreferenceTests : CommunicationTestBase
{
    private static readonly Guid TestUserId = Guid.Parse("30000000-0000-0000-0000-000000000001");

    public PreferenceTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetPreferences_ForAuthenticatedUser_Returns200()
    {
        var client = CreateAuthenticatedClient("Athlete");

        var response = await GetAsync(client, "/api/v1/preferences");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPreferences_WithoutAuth_Returns401()
    {
        var client = CreateAnonymousClient();

        var response = await GetAsync(client, "/api/v1/preferences");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePreference_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new UpdatePreferenceCommand(
            TestUserId, NotificationChannelType.Email,
            true, null, null, null);

        var response = await PutJsonAsync(client, "/api/v1/preferences", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<PreferenceDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Subscribe_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new SubscribeCommand(
            TestUserId, "Campaign",
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            NotificationChannelType.Email, "CampaignStarted");

        var response = await PostJsonAsync(client, "/api/v1/preferences/subscribe", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unsubscribe_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new UnsubscribeCommand(
            TestUserId, "Campaign",
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            NotificationChannelType.Email, "CampaignStarted");

        var response = await PostJsonAsync(client, "/api/v1/preferences/unsubscribe", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MuteChannel_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new MuteChannelCommand(TestUserId, NotificationChannelType.Email);

        var response = await PostJsonAsync(client, "/api/v1/preferences/mute", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UnmuteChannel_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new UnmuteChannelCommand(TestUserId, NotificationChannelType.Email);

        var response = await PostJsonAsync(client, "/api/v1/preferences/unmute", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MuteThenUnmute_CompletesSuccessfully()
    {
        var client = CreateAuthenticatedClient("Admin");

        var muteCmd = new MuteChannelCommand(TestUserId, NotificationChannelType.SMS);
        var muteResponse = await PostJsonAsync(client, "/api/v1/preferences/mute", muteCmd);
        muteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var unmuteCmd = new UnmuteChannelCommand(TestUserId, NotificationChannelType.SMS);
        var unmuteResponse = await PostJsonAsync(client, "/api/v1/preferences/unmute", unmuteCmd);
        unmuteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SubscribeUnsubscribe_CompletesSuccessfully()
    {
        var client = CreateAuthenticatedClient("Admin");

        var subscribeCmd = new SubscribeCommand(
            TestUserId, "Notification",
            Guid.NewGuid(), NotificationChannelType.Email, "Created");
        var subResponse = await PostJsonAsync(client, "/api/v1/preferences/subscribe", subscribeCmd);
        subResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var unsubscribeCmd = new UnsubscribeCommand(
            TestUserId, "Notification",
            Guid.NewGuid(), NotificationChannelType.Email, "Created");
        var unsubResponse = await PostJsonAsync(client, "/api/v1/preferences/unsubscribe", unsubscribeCmd);
        unsubResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
