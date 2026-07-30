using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Notification;

public class CreateNotificationCommandHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly CreateNotificationCommandHandler _handler;

    public CreateNotificationCommandHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new CreateNotificationCommandHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNotificationViaService()
    {
        var command = new CreateNotificationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationPriority.High,
            "Test Subject",
            "Test Body",
            "sender-1",
            null,
            null,
            null,
            "ext-1",
            "{}",
            new List<CreateRecipientRequest>(),
            null
        );

        var expectedDto = new NotificationDto(
            Guid.NewGuid(), command.TemplateId, command.ChannelId, "Email",
            command.ProviderId, "ProviderName", command.Priority,
            NotificationStatus.Draft, command.Subject, command.Body,
            command.SenderId, null, null, null, null, null, null,
            command.BatchId, command.CampaignId, command.ExternalId,
            command.Metadata, DateTime.UtcNow,
            new List<NotificationRecipientDto>(), new List<NotificationAttachmentDto>()
        );

        var expectedResult = Result<NotificationDto>.Success(expectedDto);
        _notificationServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateNotificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedDto);
        _notificationServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateNotificationRequest>(r =>
                r.TemplateId == command.TemplateId &&
                r.ChannelId == command.ChannelId &&
                r.ProviderId == command.ProviderId &&
                r.Priority == command.Priority &&
                r.Subject == command.Subject &&
                r.Body == command.Body &&
                r.SenderId == command.SenderId &&
                r.ScheduledAt == command.ScheduledAt &&
                r.BatchId == command.BatchId &&
                r.CampaignId == command.CampaignId &&
                r.ExternalId == command.ExternalId &&
                r.Metadata == command.Metadata &&
                r.Recipients == command.Recipients &&
                r.Attachments == command.Attachments
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapAllPropertiesCorrectly()
    {
        var recipients = new List<CreateRecipientRequest>
        {
            new(Guid.NewGuid(), "Email", "test@example.com", "Test User")
        };
        var attachments = new List<CreateAttachmentRequest>
        {
            new("file.pdf", "/path/file.pdf", "application/pdf", 1024, "S3", Guid.NewGuid())
        };
        var scheduledAt = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc);

        var command = new CreateNotificationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationPriority.Critical,
            "Critical Alert",
            "Alert body content",
            "admin",
            scheduledAt,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "ext-campaign-1",
            "{\"key\":\"value\"}",
            recipients,
            attachments
        );

        var expectedDto = new NotificationDto(
            Guid.NewGuid(), command.TemplateId, command.ChannelId, "PushNotification",
            command.ProviderId, "FCM", command.Priority,
            NotificationStatus.Draft, command.Subject, command.Body,
            command.SenderId, command.ScheduledAt, null, null, null, null, null,
            command.BatchId, command.CampaignId, command.ExternalId,
            command.Metadata, DateTime.UtcNow,
            new List<NotificationRecipientDto>(), new List<NotificationAttachmentDto>()
        );

        var expectedResult = Result<NotificationDto>.Success(expectedDto);
        _notificationServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateNotificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedDto);
        _notificationServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateNotificationRequest>(r =>
                r.TemplateId == command.TemplateId &&
                r.ChannelId == command.ChannelId &&
                r.ProviderId == command.ProviderId &&
                r.Priority == command.Priority &&
                r.Subject == command.Subject &&
                r.Body == command.Body &&
                r.SenderId == command.SenderId &&
                r.ScheduledAt == command.ScheduledAt &&
                r.BatchId == command.BatchId &&
                r.CampaignId == command.CampaignId &&
                r.ExternalId == command.ExternalId &&
                r.Metadata == command.Metadata &&
                r.Recipients == recipients &&
                r.Attachments == attachments
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WithRecipients_ShouldCreateNotificationWithRecipients()
    {
        var recipients = new List<CreateRecipientRequest>
        {
            new(Guid.NewGuid(), "Email", "user1@example.com", "User One"),
            new(Guid.NewGuid(), "SMS", "+1234567890", null)
        };

        var command = new CreateNotificationCommand(
            null,
            Guid.NewGuid(),
            null,
            NotificationPriority.Normal,
            "Bulk Notification",
            "Bulk body",
            null,
            null,
            null,
            null,
            null,
            null,
            recipients,
            null
        );

        var expectedDto = new NotificationDto(
            Guid.NewGuid(), null, command.ChannelId, "Email",
            null, null, command.Priority,
            NotificationStatus.Draft, command.Subject, command.Body,
            null, null, null, null, null, null, null,
            null, null, null, null, DateTime.UtcNow,
            new List<NotificationRecipientDto>(), new List<NotificationAttachmentDto>()
        );

        var expectedResult = Result<NotificationDto>.Success(expectedDto);
        _notificationServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateNotificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateNotificationRequest>(r => r.Recipients.Count == 2),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAttachments_ShouldCreateNotificationWithAttachments()
    {
        var attachments = new List<CreateAttachmentRequest>
        {
            new("report.pdf", "/docs/report.pdf", "application/pdf", 2048, "Local", null)
        };

        var command = new CreateNotificationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationPriority.Low,
            "Attachment Test",
            "See attached file",
            "system",
            null,
            null,
            null,
            null,
            null,
            new List<CreateRecipientRequest>(),
            attachments
        );

        var expectedDto = new NotificationDto(
            Guid.NewGuid(), command.TemplateId, command.ChannelId, "Email",
            command.ProviderId, "SMTP", command.Priority,
            NotificationStatus.Draft, command.Subject, command.Body,
            command.SenderId, null, null, null, null, null, null,
            null, null, null, null, DateTime.UtcNow,
            new List<NotificationRecipientDto>(), new List<NotificationAttachmentDto>()
        );

        var expectedResult = Result<NotificationDto>.Success(expectedDto);
        _notificationServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateNotificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateNotificationRequest>(r => r.Attachments == attachments),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ShouldReturnFailureResult()
    {
        var command = new CreateNotificationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            NotificationPriority.Normal,
            "Subject",
            "Body",
            null,
            null,
            null,
            null,
            null,
            null,
            new List<CreateRecipientRequest>(),
            null
        );

        var failureResult = Result<NotificationDto>.Failure("Service error occurred");
        _notificationServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateNotificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Service error occurred");
    }
}
