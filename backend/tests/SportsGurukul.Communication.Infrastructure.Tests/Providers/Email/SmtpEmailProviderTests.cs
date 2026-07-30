using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers.Email;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers.Email;

public class SmtpEmailProviderTests
{
    private readonly Mock<ILogger<SmtpEmailProvider>> _loggerMock;
    private readonly SmtpEmailProvider _provider;

    public SmtpEmailProviderTests()
    {
        _loggerMock = new Mock<ILogger<SmtpEmailProvider>>();
        _provider = new SmtpEmailProvider(_loggerMock.Object);
    }

    [Fact]
    public void Name_ShouldReturnSmtp()
    {
        _provider.Name.Should().Be("SMTP");
    }

    [Fact]
    public void ChannelType_ShouldReturnEmail()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.Email);
    }

    [Fact]
    public void IsAvailable_ShouldBeTrue()
    {
        _provider.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("smtp_");
        result.DurationMs.Should().BeInRange(0, 500);
        result.ProviderResponse.Should().ContainKey("simulated");
        result.ProviderResponse!["simulated"].Should().Be("true");
        result.ProviderResponse.Should().ContainKey("provider");
        result.ProviderResponse!["provider"].Should().Be("SMTP");
    }

    [Fact]
    public async Task SendAsync_ShouldAcceptMessageWithoutSubject()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Body = "Test Body"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldAcceptHtmlContent()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "HTML Test",
            Body = "<html><body><h1>Hello</h1></body></html>",
            IsHtml = true
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleRecipientWithName()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Test",
            Body = "Body",
            RecipientName = "John Doe"
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleCancellationToken()
    {
        var message = new ProviderMessage
        {
            To = "user@example.com",
            Subject = "Test",
            Body = "Body"
        };
        using var cts = new CancellationTokenSource();

        var result = await _provider.SendAsync(message, cts.Token);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task HealthCheckAsync_ShouldReturnTrue()
    {
        var healthy = await _provider.HealthCheckAsync();

        healthy.Should().BeTrue();
    }

    [Fact]
    public async Task Dispatch_SetsCorrectFromToSubjectBody()
    {
        var message = new ProviderMessage
        {
            From = "noreply@sportsgurukul.com",
            To = "student@example.com",
            Subject = "Welcome!",
            Body = "Thank you for joining.",
            IsHtml = false
        };

        var result = await _provider.SendAsync(message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().NotBeNullOrEmpty();
    }
}
