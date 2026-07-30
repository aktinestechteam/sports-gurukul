using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationDeliveryEntityTests
{
    [Fact]
    public void CreateDeliveryRecord_WithAllProperties_ShouldSetPropertiesCorrectly()
    {
        var notificationId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var delivery = new NotificationDelivery
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            RecipientId = recipientId,
            ProviderId = providerId,
            ChannelType = NotificationChannelType.SMS,
            Status = NotificationStatus.Sent,
            SentAt = now,
            DeliveredAt = null,
            ReadAt = null,
            FailedAt = null,
            FailureReason = null,
            ProviderMessageId = "msg-abc-123",
            ProviderResponse = "{\"status\":\"sent\"}",
            AttemptCount = 1,
            DurationMs = 250,
            CreatedAt = now
        };

        delivery.NotificationId.Should().Be(notificationId);
        delivery.RecipientId.Should().Be(recipientId);
        delivery.ProviderId.Should().Be(providerId);
        delivery.ChannelType.Should().Be(NotificationChannelType.SMS);
        delivery.Status.Should().Be(NotificationStatus.Sent);
        delivery.SentAt.Should().Be(now);
        delivery.DeliveredAt.Should().BeNull();
        delivery.ReadAt.Should().BeNull();
        delivery.FailedAt.Should().BeNull();
        delivery.FailureReason.Should().BeNull();
        delivery.ProviderMessageId.Should().Be("msg-abc-123");
        delivery.ProviderResponse.Should().Be("{\"status\":\"sent\"}");
        delivery.AttemptCount.Should().Be(1);
        delivery.DurationMs.Should().Be(250);
        delivery.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void StatusTracking_ShouldUpdateThroughDeliveryLifecycle()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        delivery.Status = NotificationStatus.Queued;
        delivery.Status.Should().Be(NotificationStatus.Queued);

        delivery.Status = NotificationStatus.Sending;
        delivery.Status.Should().Be(NotificationStatus.Sending);

        delivery.Status = NotificationStatus.Sent;
        delivery.Status.Should().Be(NotificationStatus.Sent);

        delivery.Status = NotificationStatus.Delivered;
        delivery.Status.Should().Be(NotificationStatus.Delivered);

        delivery.Status = NotificationStatus.Read;
        delivery.Status.Should().Be(NotificationStatus.Read);
    }

    [Fact]
    public void StatusTracking_ShouldSupportFailure()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.SMS
        };

        delivery.Status = NotificationStatus.Failed;
        delivery.FailureReason = "Provider timeout";

        delivery.Status.Should().Be(NotificationStatus.Failed);
        delivery.FailureReason.Should().Be("Provider timeout");
    }

    [Fact]
    public void TimestampAssignment_ShouldStoreSentAt()
    {
        var now = DateTime.UtcNow;

        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            SentAt = now
        };

        delivery.SentAt.Should().Be(now);
    }

    [Fact]
    public void TimestampAssignment_ShouldStoreDeliveredAt()
    {
        var now = DateTime.UtcNow;

        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            DeliveredAt = now
        };

        delivery.DeliveredAt.Should().Be(now);
    }

    [Fact]
    public void TimestampAssignment_ShouldStoreReadAt()
    {
        var now = DateTime.UtcNow;

        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            ReadAt = now
        };

        delivery.ReadAt.Should().Be(now);
    }

    [Fact]
    public void TimestampAssignment_ShouldStoreFailedAt()
    {
        var now = DateTime.UtcNow;

        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            FailedAt = now
        };

        delivery.FailedAt.Should().Be(now);
    }

    [Fact]
    public void RetryTracking_ShouldTrackAttemptCount()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        delivery.AttemptCount = 0;
        delivery.AttemptCount.Should().Be(0);

        delivery.AttemptCount = 1;
        delivery.AttemptCount.Should().Be(1);

        delivery.AttemptCount = 3;
        delivery.AttemptCount.Should().Be(3);
    }

    [Fact]
    public void RetryTracking_ShouldSupportRetriesCollection()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        delivery.Retries.Should().NotBeNull();
        delivery.Retries.Should().BeEmpty();
        delivery.Retries.Should().BeAssignableTo<ICollection<NotificationRetry>>();
    }

    [Fact]
    public void DurationMs_ShouldStoreDeliveryDuration()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            DurationMs = 1520
        };

        delivery.DurationMs.Should().Be(1520);
    }

    [Fact]
    public void ProviderMessageId_ShouldBeNull_WhenNotSet()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        delivery.ProviderMessageId.Should().BeNull();
    }

    [Fact]
    public void ProviderResponse_ShouldBeNull_WhenNotSet()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        delivery.ProviderResponse.Should().BeNull();
    }

    [Fact]
    public void DefaultAttemptCount_ShouldBeZero()
    {
        var delivery = new NotificationDelivery
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email
        };

        delivery.AttemptCount.Should().Be(0);
    }
}
