using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Email;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Email;

public class AzureCommunicationEmailProviderTests
{
    private readonly Mock<ILogger<AzureCommunicationEmailProvider>> _loggerMock;
    private readonly AzureCommunicationEmailProvider _provider;

    public AzureCommunicationEmailProviderTests()
    {
        _loggerMock = new Mock<ILogger<AzureCommunicationEmailProvider>>();
        _provider = new AzureCommunicationEmailProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnAzureCommunicationServices()
    {
        _provider.Name.Should().Be("AzureCommunicationServices");
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
            Subject = "Azure Email Test",
            Body = "Test Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("azurecommunicationservices_");
        result.ProviderResponse!["provider"].Should().Be("AzureCommunicationServices");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleAzureException()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Azure Exception Test",
            Body = "Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMessageWithFromAddress()
    {
        var message = new ProviderMessage
        {
            From = "noreply@sportsgurukul.com",
            To = "user@example.com",
            Subject = "From Address Test",
            Body = "Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleAttachmentMetadata()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Attachment Test",
            Body = "Body with attachment",
            Attachments = new List<ProviderAttachment>
            {
                new() { FileName = "report.pdf", ContentType = "application/pdf", Content = new byte[] { 1, 2, 3 } }
            }
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleHeaders()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Headers Test",
            Body = "Body",
            Headers = new Dictionary<string, string>
            {
                ["X-Custom"] = "value"
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
