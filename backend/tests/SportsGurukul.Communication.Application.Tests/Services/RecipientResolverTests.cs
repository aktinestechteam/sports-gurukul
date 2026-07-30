using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Services;

namespace SportsGurukul.Communication.Application.Tests.Services;

public class RecipientResolverTests
{
    private readonly Mock<ILogger<RecipientResolver>> _loggerMock;
    private readonly RecipientResolver _resolver;

    public RecipientResolverTests()
    {
        _loggerMock = new Mock<ILogger<RecipientResolver>>();
        _resolver = new RecipientResolver(_loggerMock.Object);
    }

    [Fact]
    public async Task ResolveAsync_ShouldReturnRecipients_WhenDestinationProvided()
    {
        var result = await _resolver.ResolveAsync(Guid.NewGuid(), "Email", "user@example.com");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().DestinationAddress.Should().Be("user@example.com");
        result.Value.First().ChannelType.Should().Be("Email");
    }

    [Fact]
    public async Task ResolveAsync_ShouldDeduplicate()
    {
        var result = await _resolver.ResolveAsync(Guid.NewGuid(), "Email", "duplicate@test.com");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task ResolveAsync_ShouldValidateContactInfo()
    {
        var result = await _resolver.ResolveAsync(null, "Email", null);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("No resolution criteria");
    }

    [Fact]
    public async Task ResolveAsync_ShouldHandleEmptyCriteria()
    {
        var result = await _resolver.ResolveAsync(null, "SMS", null);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ResolveByCriteriaAsync_ShouldReturnRecipients()
    {
        var result = await _resolver.ResolveByCriteriaAsync("age > 18 AND city = 'Mumbai'");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task ResolveAsync_ShouldReturnRecipientWithUserId()
    {
        var userId = Guid.NewGuid();
        var result = await _resolver.ResolveAsync(userId, "Email", "user@example.com");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().UserId.Should().Be(userId);
    }
}
