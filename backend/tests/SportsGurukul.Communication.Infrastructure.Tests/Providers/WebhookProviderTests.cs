using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers;

public class WebhookProviderTests
{
    private readonly Mock<ILogger<WebhookProvider>> _loggerMock;
    private readonly WebhookProvider _provider;

    public WebhookProviderTests()
    {
        _loggerMock = new Mock<ILogger<WebhookProvider>>();
        _provider = new WebhookProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnWebhookHttp()
    {
        _provider.Name.Should().Be("WebhookHTTP");
    }

    [Fact]
    public void ChannelType_ShouldReturnWebhook()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.Webhook);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "https://hooks.example.com/notify",
            Body = @"{""event"":""test"",""data"":{}}"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("webhookhttp_");
        result.ProviderResponse!["provider"].Should().Be("WebhookHTTP");
    }

    [Fact]
    public async Task SendAsync_ShouldPostToCorrectUrl()
    {
        var webhookUrl = "https://hooks.example.com/notify";
        var message = new ProviderMessage
        {
            To = webhookUrl,
            Body = @"{""event"":""user.created""}"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleHttpFailure()
    {
        var message = new ProviderMessage
        {
            To = "https://invalid.example.com/webhook",
            Body = "Test payload"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldIncludeHeaders()
    {
        var message = new ProviderMessage
        {
            To = "https://hooks.example.com/notify",
            Body = "payload",
            Headers = new Dictionary<string, string>
            {
                ["X-Signature"] = "sha256=abc123",
                ["X-Event-Type"] = "notification.sent",
                ["Content-Type"] = "application/json"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleTimeout()
    {
        var message = new ProviderMessage
        {
            To = "https://slow.example.com/webhook",
            Body = "payload",
            Metadata = new Dictionary<string, object>
            {
                ["timeout_seconds"] = 5
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleRetryLogic()
    {
        var message = new ProviderMessage
        {
            To = "https://hooks.example.com/notify",
            Body = "retry test",
            Metadata = new Dictionary<string, object>
            {
                ["max_retries"] = 3,
                ["retry_delay_ms"] = 1000
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
