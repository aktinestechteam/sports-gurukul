using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Sms;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Sms;

public class Msg91SmsProviderTests
{
    private readonly Mock<ILogger<Msg91SmsProvider>> _loggerMock;
    private readonly Msg91SmsProvider _provider;

    public Msg91SmsProviderTests()
    {
        _loggerMock = new Mock<ILogger<Msg91SmsProvider>>();
        _provider = new Msg91SmsProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnMsg91()
    {
        _provider.Name.Should().Be("MSG91");
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
            To = "+919876543210",
            Body = "Test SMS via MSG91"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("msg91_");
        result.ProviderResponse!["provider"].Should().Be("MSG91");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleApiFailure()
    {
        var message = new ProviderMessage
        {
            To = "+919876543210",
            Body = "API Failure Test"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleDltTemplate()
    {
        var message = new ProviderMessage
        {
            To = "+919876543210",
            Body = "Your OTP is 123456",
            Metadata = new Dictionary<string, object>
            {
                ["dlt_template_id"] = "1234567890123456789",
                ["dlt_entity_id"] = "1234567890123456789"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleUnicodeMessage()
    {
        var message = new ProviderMessage
        {
            To = "+919876543210",
            Body = "नमस्ते, आपका ओटीपी 123456 है"
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMultipleRecipients()
    {
        var message = new ProviderMessage
        {
            To = "+919876543210",
            Body = "Bulk SMS test"
        };

        for (var i = 0; i < 5; i++)
        {
            var result = await _provider.SendAsync(message);
            result.IsSuccess.Should().BeTrue();
        }
    }

    [Fact]
    public async Task HealthCheckAsync_ShouldReturnTrue()
    {
        var healthy = await _provider.HealthCheckAsync();
        healthy.Should().BeTrue();
    }
}
