using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationEntityTests
{
    [Fact]
    public void CreateNotification_WithAllRequiredProperties_ShouldSetPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var notification = new Notification
        {
            Id = id,
            TemplateId = templateId,
            ChannelId = channelId,
            ProviderId = providerId,
            Priority = NotificationPriority.High,
            Status = NotificationStatus.Draft,
            Subject = "Welcome to SportsGurukul",
            Body = "Thank you for joining!",
            SenderId = "system",
            ScheduledAt = now.AddHours(1),
            SentAt = null,
            DeliveredAt = null,
            ReadAt = null,
            FailedAt = null,
            FailureReason = null,
            ErrorCode = null,
            BatchId = batchId,
            CampaignId = campaignId,
            ExternalId = "ext-001",
            Metadata = "{\"source\":\"registration\"}",
            CreatedAt = now
        };

        notification.Id.Should().Be(id);
        notification.TemplateId.Should().Be(templateId);
        notification.ChannelId.Should().Be(channelId);
        notification.ProviderId.Should().Be(providerId);
        notification.Priority.Should().Be(NotificationPriority.High);
        notification.Status.Should().Be(NotificationStatus.Draft);
        notification.Subject.Should().Be("Welcome to SportsGurukul");
        notification.Body.Should().Be("Thank you for joining!");
        notification.SenderId.Should().Be("system");
        notification.ScheduledAt.Should().Be(now.AddHours(1));
        notification.SentAt.Should().BeNull();
        notification.DeliveredAt.Should().BeNull();
        notification.ReadAt.Should().BeNull();
        notification.FailedAt.Should().BeNull();
        notification.FailureReason.Should().BeNull();
        notification.ErrorCode.Should().BeNull();
        notification.BatchId.Should().Be(batchId);
        notification.CampaignId.Should().Be(campaignId);
        notification.ExternalId.Should().Be("ext-001");
        notification.Metadata.Should().Be("{\"source\":\"registration\"}");
        notification.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void DefaultStatus_ShouldBeDraft()
    {
        var notification = new Notification();

        notification.Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public void StatusTransitions_FromDraftToQueuedToSendingToSentToDeliveredToRead_ShouldUpdateStatus()
    {
        var notification = new Notification();
        notification.Status.Should().Be(NotificationStatus.Draft);

        notification.Status = NotificationStatus.Queued;
        notification.Status.Should().Be(NotificationStatus.Queued);

        notification.Status = NotificationStatus.Sending;
        notification.Status.Should().Be(NotificationStatus.Sending);

        notification.Status = NotificationStatus.Sent;
        notification.Status.Should().Be(NotificationStatus.Sent);

        notification.Status = NotificationStatus.Delivered;
        notification.Status.Should().Be(NotificationStatus.Delivered);

        notification.Status = NotificationStatus.Read;
        notification.Status.Should().Be(NotificationStatus.Read);
    }

    [Fact]
    public void StatusTransition_QueuedToFailed_ShouldUpdateStatus()
    {
        var notification = new Notification { Status = NotificationStatus.Queued };

        notification.Status = NotificationStatus.Failed;

        notification.Status.Should().Be(NotificationStatus.Failed);
        notification.FailedAt = DateTime.UtcNow;
        notification.FailedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void StatusTransition_QueuedToCancelled_ShouldUpdateStatus()
    {
        var notification = new Notification { Status = NotificationStatus.Queued };

        notification.Status = NotificationStatus.Cancelled;

        notification.Status.Should().Be(NotificationStatus.Cancelled);
    }

    [Fact]
    public void StatusTransition_SendingToFailed_ShouldUpdateStatus()
    {
        var notification = new Notification { Status = NotificationStatus.Sending };

        notification.Status = NotificationStatus.Failed;

        notification.Status.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public void CannotTransition_FromCancelled_ShouldAllowStatusChange()
    {
        var notification = new Notification { Status = NotificationStatus.Cancelled };

        notification.Status = NotificationStatus.Queued;

        notification.Status.Should().Be(NotificationStatus.Queued);
    }

    [Fact]
    public void CannotTransition_FromExpired_ShouldAllowStatusChange()
    {
        var notification = new Notification { Status = NotificationStatus.Expired };

        notification.Status = NotificationStatus.Draft;

        notification.Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public void CreatedAt_ShouldBeSetOnCreation()
    {
        var now = DateTime.UtcNow;

        var notification = new Notification { CreatedAt = now };

        notification.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void UpdatedAt_ShouldUpdateOnChanges()
    {
        var now = DateTime.UtcNow;
        var later = now.AddMinutes(5);
        var notification = new Notification { CreatedAt = now };

        notification.UpdatedAt = later;

        notification.UpdatedAt.Should().Be(later);
    }

    [Fact]
    public void Notification_WithRecipients_ShouldInitializeCollection()
    {
        var notification = new Notification();

        notification.Recipients.Should().NotBeNull();
        notification.Recipients.Should().BeEmpty();
        notification.Recipients.Should().BeAssignableTo<ICollection<NotificationRecipient>>();
    }

    [Fact]
    public void Notification_WithAttachments_ShouldInitializeCollection()
    {
        var notification = new Notification();

        notification.Attachments.Should().NotBeNull();
        notification.Attachments.Should().BeEmpty();
        notification.Attachments.Should().BeAssignableTo<ICollection<NotificationAttachment>>();
    }

    [Fact]
    public void Notification_WithDeliveries_ShouldInitializeCollection()
    {
        var notification = new Notification();

        notification.Deliveries.Should().NotBeNull();
        notification.Deliveries.Should().BeEmpty();
        notification.Deliveries.Should().BeAssignableTo<ICollection<NotificationDelivery>>();
    }

    [Fact]
    public void RowVersion_ShouldBeEmptyArray_ByDefault()
    {
        var notification = new Notification();

        notification.RowVersion.Should().NotBeNull();
        notification.RowVersion.Should().BeEmpty();
    }

    [Fact]
    public void Priority_ShouldDefaultToLow()
    {
        var notification = new Notification();

        notification.Priority.Should().Be(NotificationPriority.Low);
    }

    [Fact]
    public void FailureReason_ShouldBeNull_WhenNotSet()
    {
        var notification = new Notification();

        notification.FailureReason.Should().BeNull();
    }

    [Fact]
    public void ErrorCode_ShouldBeNull_WhenNotSet()
    {
        var notification = new Notification();

        notification.ErrorCode.Should().BeNull();
    }
}
