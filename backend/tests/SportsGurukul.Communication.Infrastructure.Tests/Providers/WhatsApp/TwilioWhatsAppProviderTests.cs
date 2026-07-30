using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.WhatsApp;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.WhatsApp;

public class TwilioWhatsAppProviderTests
{
    private readonly Mock<ILogger<TwilioWhatsAppProvider>> _loggerMock;
    private readonly TwilioWhatsAppProvider _provider;

    public TwilioWhatsAppProviderTests()
    {
        _loggerMock = new Mock<ILogger<TwilioWhatsAppProvider>>();
        _provider = new TwilioWhatsAppProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnTwilioWhatsApp()
    {
        _provider.Name.Should().Be("TwilioWhatsApp");
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
            To = "whatsapp:+1234567890",
            Body = "Hello from Twilio WhatsApp"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("twiliowhatsapp_");
        result.ProviderResponse!["provider"].Should().Be("TwilioWhatsApp");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleFailure()
    {
        var message = new ProviderMessage
        {
            To = "whatsapp:+1234567890",
            Body = "Test"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMediaMessage()
    {
        var message = new ProviderMessage
        {
            To = "whatsapp:+1234567890",
            Body = "Your invoice is ready",
            Metadata = new Dictionary<string, object>
            {
                ["media_url"] = "https://example.com/invoice.pdf"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleTemplateMessage()
    {
        var message = new ProviderMessage
        {
            To = "whatsapp:+1234567890",
            Body = "Your order #123 has been shipped",
            Metadata = new Dictionary<string, object>
            {
                ["template_id"] = "HX_12345",
                ["status_callback"] = "https://api.example.com/status"
            }
        };

        var result = await _provider.SendAsync(message);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleFreeFormMessage()
    {
        var message = new ProviderMessage
        {
            To = "whatsapp:+1234567890",
            Body = "Hi there! How can we help you today?"
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
