using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationRecipientEntityTests
{
    [Fact]
    public void CreateRecipient_WithEmail_ShouldSetPropertiesCorrectly()
    {
        var notificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var recipient = new NotificationRecipient
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            UserId = userId,
            ChannelType = NotificationChannelType.Email,
            DestinationAddress = "user@example.com",
            RecipientName = "John Doe",
            Status = NotificationStatus.Draft,
            SentAt = null,
            DeliveredAt = null,
            ReadAt = null,
            FailedAt = null,
            FailureReason = null,
            IsRead = false,
            ReadAtTimestamp = null,
            CreatedAt = now
        };

        recipient.NotificationId.Should().Be(notificationId);
        recipient.UserId.Should().Be(userId);
        recipient.ChannelType.Should().Be(NotificationChannelType.Email);
        recipient.DestinationAddress.Should().Be("user@example.com");
        recipient.RecipientName.Should().Be("John Doe");
        recipient.Status.Should().Be(NotificationStatus.Draft);
        recipient.SentAt.Should().BeNull();
        recipient.DeliveredAt.Should().BeNull();
        recipient.ReadAt.Should().BeNull();
        recipient.FailedAt.Should().BeNull();
        recipient.FailureReason.Should().BeNull();
        recipient.IsRead.Should().BeFalse();
        recipient.ReadAtTimestamp.Should().BeNull();
        recipient.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void CreateRecipient_WithPhone_ShouldSetPropertiesCorrectly()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.SMS,
            DestinationAddress = "+919876543210"
        };

        recipient.ChannelType.Should().Be(NotificationChannelType.SMS);
        recipient.DestinationAddress.Should().Be("+919876543210");
    }

    [Fact]
    public void DeliveryChannelAssignment_ShouldSupportEmail()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            DestinationAddress = "test@example.com"
        };

        recipient.ChannelType.Should().Be(NotificationChannelType.Email);
    }

    [Fact]
    public void DeliveryChannelAssignment_ShouldSupportSMS()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.SMS,
            DestinationAddress = "+1234567890"
        };

        recipient.ChannelType.Should().Be(NotificationChannelType.SMS);
    }

    [Fact]
    public void DeliveryChannelAssignment_ShouldSupportWhatsApp()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.WhatsApp,
            DestinationAddress = "+919876543210"
        };

        recipient.ChannelType.Should().Be(NotificationChannelType.WhatsApp);
    }

    [Fact]
    public void DeliveryChannelAssignment_ShouldSupportPushNotification()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.PushNotification,
            DestinationAddress = "device-token-xyz"
        };

        recipient.ChannelType.Should().Be(NotificationChannelType.PushNotification);
    }

    [Fact]
    public void DeliveryChannelAssignment_ShouldSupportInAppNotification()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.InAppNotification,
            DestinationAddress = "user-123"
        };

        recipient.ChannelType.Should().Be(NotificationChannelType.InAppNotification);
    }

    [Fact]
    public void DeliveryChannelAssignment_ShouldSupportWebhook()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Webhook,
            DestinationAddress = "https://hooks.example.com/notify"
        };

        recipient.ChannelType.Should().Be(NotificationChannelType.Webhook);
    }

    [Fact]
    public void IsRead_ShouldDefaultToFalse()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            DestinationAddress = "user@example.com"
        };

        recipient.IsRead.Should().BeFalse();
    }

    [Fact]
    public void Status_ShouldDefaultToDraft()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            DestinationAddress = "user@example.com"
        };

        recipient.Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public void RecipientName_ShouldBeNull_WhenNotSet()
    {
        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            DestinationAddress = "user@example.com"
        };

        recipient.RecipientName.Should().BeNull();
    }

    [Fact]
    public void ReadAtTimestamp_ShouldBeSet_WhenNotificationIsRead()
    {
        var now = DateTime.UtcNow;

        var recipient = new NotificationRecipient
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            DestinationAddress = "user@example.com",
            IsRead = true,
            ReadAtTimestamp = now
        };

        recipient.IsRead.Should().BeTrue();
        recipient.ReadAtTimestamp.Should().Be(now);
    }
}
