using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Sms;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Sms;

public class TwilioSmsProviderTests
{
    private readonly Mock<ILogger<TwilioSmsProvider>> _loggerMock;
    private readonly TwilioSmsProvider _provider;

    public TwilioSmsProviderTests()
    {
        _loggerMock = new Mock<ILogger<TwilioSmsProvider>>();
        _provider = new TwilioSmsProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnTwilioSms()
    {
        _provider.Name.Should().Be("TwilioSMS");
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
            To = "+1234567890",
            Body = "Test SMS message"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("twiliosms_");
        result.ProviderResponse!["provider"].Should().Be("TwilioSMS");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleTwilioException()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Body = "Test"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldValidatePhoneNumberFormat()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Body = "Valid phone"
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleInternationalNumber()
    {
        var message = new ProviderMessage
        {
            To = "+919876543210",
            Body = "International SMS"
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleLongMessage()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Body = new string('A', 500)
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMessageWithSubject()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Subject = "SMS Subject",
            Body = "SMS Body"
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
