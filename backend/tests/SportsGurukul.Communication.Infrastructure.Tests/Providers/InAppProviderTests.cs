using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers;

public class InAppProviderTests
{
    private readonly Mock<ILogger<InAppProvider>> _loggerMock;
    private readonly InAppProvider _provider;

    public InAppProviderTests()
    {
        _loggerMock = new Mock<ILogger<InAppProvider>>();
        _provider = new InAppProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnSignalRInApp()
    {
        _provider.Name.Should().Be("SignalRInApp");
    }

    [Fact]
    public void ChannelType_ShouldReturnInAppNotification()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.InAppNotification);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "user-123",
            Body = "You have a new notification"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("signalrinapp_");
        result.ProviderResponse!["provider"].Should().Be("SignalRInApp");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleInAppSpecificPayload()
    {
        var message = new ProviderMessage
        {
            To = "user-456",
            Subject = "New Message",
            Body = "John sent you a message",
            Metadata = new Dictionary<string, object>
            {
                ["type"] = "chat_message",
                ["sender_id"] = "user-789",
                ["conversation_id"] = "conv-001",
                ["action_url"] = "/messages/conv-001"
            }
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleNotificationWithActions()
    {
        var message = new ProviderMessage
        {
            To = "user-123",
            Subject = "Friend Request",
            Body = "Jane Doe sent you a friend request",
            Metadata = new Dictionary<string, object>
            {
                ["actions"] = new[]
                {
                    new { label = "Accept", action = "/friends/accept/123" },
                    new { label = "Decline", action = "/friends/decline/123" }
                }
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleRealTimeDelivery()
    {
        var message = new ProviderMessage
        {
            To = "user-123",
            Subject = "Live Score Update",
            Body = "India: 245/3 (40 overs)",
            Metadata = new Dictionary<string, object>
            {
                ["priority"] = "high",
                ["persistent"] = true
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleGroupNotification()
    {
        var message = new ProviderMessage
        {
            To = "group-team-a",
            Body = "Match starts in 30 minutes",
            Metadata = new Dictionary<string, object>
            {
                ["group"] = true,
                ["members"] = 11
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task HealthCheckAsync_ShouldReturnTrue()
    {
        var healthy = await _provider.HealthCheckAsync();
        healthy.Should().BeTrue();
    }
}
