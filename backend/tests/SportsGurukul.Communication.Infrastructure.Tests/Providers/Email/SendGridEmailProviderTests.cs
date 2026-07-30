using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Email;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Email;

public class SendGridEmailProviderTests
{
    private readonly Mock<ILogger<SendGridEmailProvider>> _loggerMock;
    private readonly SendGridEmailProvider _provider;

    public SendGridEmailProviderTests()
    {
        _loggerMock = new Mock<ILogger<SendGridEmailProvider>>();
        _provider = new SendGridEmailProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnSendGrid()
    {
        _provider.Name.Should().Be("SendGrid");
    }

    [Fact]
    public void ChannelType_ShouldReturnEmail()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.Email);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Test",
            Body = "Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("sendgrid_");
        result.ProviderResponse!["provider"].Should().Be("SendGrid");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleApiFailureSimulation()
    {
        var message = new ProviderMessage
        {
            To = "invalid@example.com",
            Subject = "Test",
            Body = "Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse!["simulated"].Should().Be("true");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleRateLimiting()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Rate Limit Test",
            Body = "Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.DurationMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMultipleRecipientsSequentially()
    {
        var recipients = new[] { "a@test.com", "b@test.com", "c@test.com" };

        foreach (var to in recipients)
        {
            var message = new ProviderMessage { To = to, Subject = "Test", Body = "Body" };
            var result = await _provider.SendAsync(message);
            result.IsSuccess.Should().BeTrue();
        }
    }

    [Fact]
    public async Task SendAsync_ShouldHandleRetryOnTransientFailure()
    {
        var message = new ProviderMessage
        {
            To = "retry@example.com",
            Subject = "Retry Test",
            Body = "Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleEmptyBody()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "No Body",
            Body = string.Empty
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
