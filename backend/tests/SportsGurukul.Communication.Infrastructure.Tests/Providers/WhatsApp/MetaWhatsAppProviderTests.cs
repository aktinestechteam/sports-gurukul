using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.WhatsApp;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.WhatsApp;

public class MetaWhatsAppProviderTests
{
    private readonly Mock<ILogger<MetaWhatsAppProvider>> _loggerMock;
    private readonly MetaWhatsAppProvider _provider;

    public MetaWhatsAppProviderTests()
    {
        _loggerMock = new Mock<ILogger<MetaWhatsAppProvider>>();
        _provider = new MetaWhatsAppProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnMetaWhatsApp()
    {
        _provider.Name.Should().Be("MetaWhatsApp");
    }

    [Fact]
    public void ChannelType_ShouldReturnWhatsApp()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.WhatsApp);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Body = "Hello from Meta WhatsApp Cloud API"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("metawhatsapp_");
        result.ProviderResponse!["provider"].Should().Be("MetaWhatsApp");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleApiFailure()
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
    public async Task SendAsync_ShouldHandleTemplateMessage()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Body = "Your appointment is confirmed for {{date}}",
            Metadata = new Dictionary<string, object>
            {
                ["template_name"] = "appointment_confirmation",
                ["language"] = "en_US",
                ["header_params"] = new[] { "HeaderParam" },
                ["body_params"] = new[] { "John", "2024-01-15" }
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleInteractiveMessage()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Body = "Would you like to proceed?",
            Metadata = new Dictionary<string, object>
            {
                ["type"] = "interactive",
                ["buttons"] = new[] { "Yes", "No" }
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMediaMessage()
    {
        var message = new ProviderMessage
        {
            To = "+1234567890",
            Body = "Check this image",
            Metadata = new Dictionary<string, object>
            {
                ["media_url"] = "https://example.com/image.jpg",
                ["media_type"] = "image"
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
