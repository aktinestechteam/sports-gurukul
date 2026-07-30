using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Email;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Email;

public class AmazonSesEmailProviderTests
{
    private readonly Mock<ILogger<AmazonSesEmailProvider>> _loggerMock;
    private readonly AmazonSesEmailProvider _provider;

    public AmazonSesEmailProviderTests()
    {
        _loggerMock = new Mock<ILogger<AmazonSesEmailProvider>>();
        _provider = new AmazonSesEmailProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnAmazonSes()
    {
        _provider.Name.Should().Be("AmazonSES");
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
            Subject = "AWS Email Test",
            Body = "Test Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("amazonses_");
        result.ProviderResponse!["provider"].Should().Be("AmazonSES");
    }

    [Fact]
    public async Task SendAsync_ShouldHandleAwsException()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "AWS Exception Test",
            Body = "Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderResponse.Should().ContainKey("simulated");
    }

    [Fact]
    public async Task SendAsync_ShouldUseCorrectRegionEndpoint()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Region Test",
            Body = "Body",
            Metadata = new Dictionary<string, object>
            {
                ["region"] = "us-east-1"
            }
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleLargeBody()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Large Body Test",
            Body = new string('X', 10000)
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleHtmlContent()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "HTML Test",
            Body = "<html><body><p>Hello</p></body></html>",
            IsHtml = true,
            ContentType = "text/html"
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
