using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.BusinessRules;

public class QuietHoursRuleTests
{
    private readonly Mock<IPreferenceRepository> _preferenceRepoMock;
    private readonly Mock<ILogger<QuietHoursRule>> _loggerMock;
    private readonly QuietHoursRule _rule;

    public QuietHoursRuleTests()
    {
        _preferenceRepoMock = new Mock<IPreferenceRepository>();
        _loggerMock = new Mock<ILogger<QuietHoursRule>>();
        _rule = new QuietHoursRule(_preferenceRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_DuringBusinessHours_ReturnsSuccess()
    {
        var now = TimeOnly.FromDateTime(DateTime.UtcNow);
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QuietHoursStart = now.AddHours(2),
            QuietHoursEnd = now.AddHours(4),
            IsEnabled = true
        };
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        var result = await _rule.ValidateAsync(CreateRequest());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_DuringQuietHours_ReturnsFailure()
    {
        var now = TimeOnly.FromDateTime(DateTime.UtcNow);
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QuietHoursStart = now.AddHours(-1),
            QuietHoursEnd = now.AddHours(1),
            IsEnabled = true
        };
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        var result = await _rule.ValidateAsync(CreateRequest());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("quiet hours");
    }

    [Fact]
    public async Task ValidateAsync_WhenQuietHoursDisabled_ReturnsSuccess()
    {
        var preference = new NotificationPreference
        {
            UserId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QuietHoursStart = null,
            QuietHoursEnd = null,
            IsEnabled = true
        };
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        var result = await _rule.ValidateAsync(CreateRequest());

        result.IsSuccess.Should().BeTrue();
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
    public async Task ValidateAsync_WhenPreferenceNotFound_ReturnsSuccess()
    {
        _preferenceRepoMock
            .Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationPreference?)null);

        var result = await _rule.ValidateAsync(CreateRequest());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenNotCreateNotificationRequest_ReturnsSuccess()
    {
        var result = await _rule.ValidateAsync(new object());

        result.IsSuccess.Should().BeTrue();
    }

    private static CreateNotificationRequest CreateRequest() =>
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
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );
}
