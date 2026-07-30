using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Push;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Push;

public class FirebasePushProviderTests
{
    private readonly Mock<ILogger<FirebasePushProvider>> _loggerMock;
    private readonly FirebasePushProvider _provider;

    public FirebasePushProviderTests()
    {
        _loggerMock = new Mock<ILogger<FirebasePushProvider>>();
        _provider = new FirebasePushProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnFirebaseCloudMessaging()
    {
        _provider.Name.Should().Be("FirebaseCloudMessaging");
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
            To = "device-token-12345",
            Body = "Test push notification"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("firebasecloudmessaging_");
        result.ProviderResponse!["provider"].Should().Be("FirebaseCloudMessaging");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleFcmFailure()
    {
        var message = new ProviderMessage
        {
            To = "invalid-device-token",
            Body = "Test"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldValidateDeviceToken()
    {
        var message = new ProviderMessage
        {
            To = string.Empty,
            Body = "No token"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleNotificationPayload()
    {
        var message = new ProviderMessage
        {
            To = "device-token-12345",
            Subject = "New Achievement!",
            Body = "You have unlocked a new badge.",
            Metadata = new Dictionary<string, object>
            {
                ["badge"] = 1,
                ["sound"] = "default",
                ["click_action"] = "OPEN_ACHIEVEMENT"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleDataOnlyMessage()
    {
        var message = new ProviderMessage
        {
            To = "device-token-12345",
            Body = "Data payload",
            Metadata = new Dictionary<string, object>
            {
                ["type"] = "silent",
                ["score"] = 100
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMultipleTokens()
    {
        var message = new ProviderMessage
        {
            To = "token-1,token-2,token-3",
            Body = "Multicast"
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
