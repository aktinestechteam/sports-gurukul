using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Push;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Push;

public class ApplePushProviderTests
{
    private readonly Mock<ILogger<ApplePushProvider>> _loggerMock;
    private readonly ApplePushProvider _provider;

    public ApplePushProviderTests()
    {
        _loggerMock = new Mock<ILogger<ApplePushProvider>>();
        _provider = new ApplePushProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnApplePushNotification()
    {
        _provider.Name.Should().Be("ApplePushNotification");
    }

    [Fact]
    public void ChannelType_ShouldReturnPushNotification()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.PushNotification);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "apns-device-token-hex",
            Body = "Test push via APNs"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("applepushnotification_");
        result.ProviderResponse!["provider"].Should().Be("ApplePushNotification");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleApnsFailure()
    {
        var message = new ProviderMessage
        {
            To = "invalid-token",
            Body = "Test"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleNotificationWithBadge()
    {
        var message = new ProviderMessage
        {
            To = "apns-device-token-hex",
            Subject = "New Message",
            Body = "You have a new message",
            Metadata = new Dictionary<string, object>
            {
                ["badge"] = 5,
                ["sound"] = "chime.aiff",
                ["category"] = "MESSAGE"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleVoipNotification()
    {
        var message = new ProviderMessage
        {
            To = "apns-device-token-hex",
            Body = "Incoming call",
            Metadata = new Dictionary<string, object>
            {
                ["push_type"] = "voip",
                ["caller"] = "John Doe"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleSandboxEnvironment()
    {
        var message = new ProviderMessage
        {
            To = "apns-device-token-hex",
            Body = "Sandbox test",
            Metadata = new Dictionary<string, object>
            {
                ["environment"] = "sandbox"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleExpiry()
    {
        var message = new ProviderMessage
        {
            To = "apns-device-token-hex",
            Body = "Expiry test",
            Metadata = new Dictionary<string, object>
            {
                ["expiry"] = 3600
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
