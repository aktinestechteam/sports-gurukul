using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Sms;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Sms;

public class TextLocalSmsProviderTests
{
    private readonly Mock<ILogger<TextLocalSmsProvider>> _loggerMock;
    private readonly TextLocalSmsProvider _provider;

    public TextLocalSmsProviderTests()
    {
        _loggerMock = new Mock<ILogger<TextLocalSmsProvider>>();
        _provider = new TextLocalSmsProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnTextLocal()
    {
        _provider.Name.Should().Be("TextLocal");
    }

    [Fact]
    public void ChannelType_ShouldReturnSms()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.SMS);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "+447123456789",
            Body = "Test SMS via TextLocal"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("textlocal_");
        result.ProviderResponse!["provider"].Should().Be("TextLocal");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleApiFailure()
    {
        var message = new ProviderMessage
        {
            To = "+447123456789",
            Body = "API Failure Test"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleSenderName()
    {
        var message = new ProviderMessage
        {
            To = "+447123456789",
            Body = "With Sender Name",
            From = "SportsGuru"
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleLongMessage()
    {
        var message = new ProviderMessage
        {
            To = "+447123456789",
            Body = new string('X', 765)
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleEmptyBody()
    {
        var message = new ProviderMessage
        {
            To = "+447123456789",
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
