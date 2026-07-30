using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.BusinessRules;

public class RateLimitRuleTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IPreferenceRepository> _preferenceRepoMock;
    private readonly Mock<ILogger<RateLimitRule>> _loggerMock;
    private readonly RateLimitRule _rule;

    public RateLimitRuleTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _preferenceRepoMock = new Mock<IPreferenceRepository>();
        _loggerMock = new Mock<ILogger<RateLimitRule>>();
        _rule = new RateLimitRule(_notificationRepoMock.Object, _preferenceRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_WhenUnderRateLimit_ReturnsSuccess()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            MaxPerDay = 10,
            IsEnabled = true
        };
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _notificationRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification> { new() });

        var result = await _rule.ValidateAsync(CreateRequest());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenOverRateLimit_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var preference = new NotificationPreference
        {
            UserId = userId,
            ChannelType = NotificationChannelType.Email,
            MaxPerDay = 2,
            IsEnabled = true
        };
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        var todayNotifications = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
        };
        _notificationRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todayNotifications);

        var result = await _rule.ValidateAsync(CreateRequest(userId: userId));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Rate limit");
    }

    [Fact]
    public async Task ValidateAsync_WhenRecipientHasNoUserId_ReturnsSuccess()
    {
        var request = new CreateNotificationRequest(
            TemplateId: null,
            ChannelId: Guid.NewGuid(),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test",
            Body: "Test",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: null, ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenMaxPerDayIsNull_ReturnsSuccess()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            MaxPerDay = null,
            IsEnabled = true
        };
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        var result = await _rule.ValidateAsync(CreateRequest());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenNotCreateNotificationRequest_ReturnsSuccess()
    {
        var result = await _rule.ValidateAsync(new object());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ResetsAfterTimeWindow()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            MaxPerDay = 1,
            IsEnabled = true
        };
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _notificationRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var result = await _rule.ValidateAsync(CreateRequest());

        result.IsSuccess.Should().BeTrue();
    }

    private static CreateNotificationRequest CreateRequest(Guid? userId = null) =>
        new(
            TemplateId: null,
            ChannelId: Guid.NewGuid(),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test Subject",
            Body: "Test Body",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: userId ?? Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );
}
