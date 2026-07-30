using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationQueueEntityTests
{
    [Fact]
    public void CreateQueueItem_WithPriority_ShouldSetPropertiesCorrectly()
    {
        var notificationId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var queueItem = new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            ChannelType = NotificationChannelType.Email,
            Status = NotificationStatus.Queued,
            Priority = NotificationPriority.High,
            QueuedAt = now,
            ProcessStartedAt = null,
            ProcessCompletedAt = null,
            AttemptCount = 0,
            MaxAttempts = 3,
            NextAttemptAt = now,
            LockExpiresAt = null,
            LockToken = null,
            BatchId = batchId,
            CreatedAt = now
        };

        queueItem.NotificationId.Should().Be(notificationId);
        queueItem.ChannelType.Should().Be(NotificationChannelType.Email);
        queueItem.Status.Should().Be(NotificationStatus.Queued);
        queueItem.Priority.Should().Be(NotificationPriority.High);
        queueItem.QueuedAt.Should().Be(now);
        queueItem.ProcessStartedAt.Should().BeNull();
        queueItem.ProcessCompletedAt.Should().BeNull();
        queueItem.AttemptCount.Should().Be(0);
        queueItem.MaxAttempts.Should().Be(3);
        queueItem.NextAttemptAt.Should().Be(now);
        queueItem.LockExpiresAt.Should().BeNull();
        queueItem.LockToken.Should().BeNull();
        queueItem.BatchId.Should().Be(batchId);
        queueItem.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void PriorityOrdering_Low_ShouldHaveCorrectOrder()
    {
        var low = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            Priority = NotificationPriority.Low
        };

        low.Priority.Should().Be(NotificationPriority.Low);
        ((int)low.Priority).Should().Be(0);
    }

    [Fact]
    public void PriorityOrdering_Normal_ShouldHaveCorrectOrder()
    {
        var normal = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            Priority = NotificationPriority.Normal
        };

        normal.Priority.Should().Be(NotificationPriority.Normal);
        ((int)normal.Priority).Should().Be(1);
    }

    [Fact]
    public void PriorityOrdering_High_ShouldHaveCorrectOrder()
    {
        var high = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            Priority = NotificationPriority.High
        };

        high.Priority.Should().Be(NotificationPriority.High);
        ((int)high.Priority).Should().Be(2);
    }

    [Fact]
    public void PriorityOrdering_Critical_ShouldHaveCorrectOrder()
    {
        var critical = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            Priority = NotificationPriority.Critical
        };

        critical.Priority.Should().Be(NotificationPriority.Critical);
        ((int)critical.Priority).Should().Be(3);
    }

    [Fact]
    public void PriorityOrdering_LowToCritical_ShouldBeAscending()
    {
        var low = new NotificationQueue { NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Priority = NotificationPriority.Low };
        var normal = new NotificationQueue { NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Priority = NotificationPriority.Normal };
        var high = new NotificationQueue { NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Priority = NotificationPriority.High };
        var critical = new NotificationQueue { NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Priority = NotificationPriority.Critical };

        ((int)low.Priority).Should().BeLessThan((int)normal.Priority);
        ((int)normal.Priority).Should().BeLessThan((int)high.Priority);
        ((int)high.Priority).Should().BeLessThan((int)critical.Priority);
    }

    [Fact]
    public void AttemptCounter_ShouldStartAtZero()
    {
        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = DateTime.UtcNow
        };

        queueItem.AttemptCount.Should().Be(0);
    }

    [Fact]
    public void AttemptCounter_ShouldIncrement()
    {
        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = DateTime.UtcNow,
            AttemptCount = 1
        };

        queueItem.AttemptCount.Should().Be(1);

        queueItem.AttemptCount = 2;
        queueItem.AttemptCount.Should().Be(2);

        queueItem.AttemptCount = 3;
        queueItem.AttemptCount.Should().Be(3);
    }

    [Fact]
    public void MaxAttempts_ShouldDefaultToThree()
    {
        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = DateTime.UtcNow
        };

        queueItem.MaxAttempts.Should().Be(3);
    }

    [Fact]
    public void MaxAttempts_ShouldBeConfigurable()
    {
        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = DateTime.UtcNow,
            MaxAttempts = 5
        };

        queueItem.MaxAttempts.Should().Be(5);
    }

    [Fact]
    public void ScheduledDeliveryTime_ShouldStoreNextAttemptAt()
    {
        var future = DateTime.UtcNow.AddHours(1);

        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = DateTime.UtcNow,
            NextAttemptAt = future
        };

        queueItem.NextAttemptAt.Should().Be(future);
    }

    [Fact]
    public void LockToken_ShouldBeNull_WhenNotLocked()
    {
        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = DateTime.UtcNow
        };

        queueItem.LockToken.Should().BeNull();
        queueItem.LockExpiresAt.Should().BeNull();
    }

    [Fact]
    public void LockQueueItem_ShouldSetLockProperties()
    {
        var now = DateTime.UtcNow;

        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = now,
            LockToken = "lock-token-abc",
            LockExpiresAt = now.AddMinutes(5)
        };

        queueItem.LockToken.Should().Be("lock-token-abc");
        queueItem.LockExpiresAt.Should().Be(now.AddMinutes(5));
    }

    [Fact]
    public void ProcessTimestamps_ShouldTrackProcessing()
    {
        var now = DateTime.UtcNow;

        var queueItem = new NotificationQueue
        {
            NotificationId = Guid.NewGuid(),
            ChannelType = NotificationChannelType.Email,
            QueuedAt = now,
            ProcessStartedAt = now.AddSeconds(5),
            ProcessCompletedAt = now.AddSeconds(10)
        };

        queueItem.ProcessStartedAt.Should().Be(now.AddSeconds(5));
        queueItem.ProcessCompletedAt.Should().Be(now.AddSeconds(10));
    }
}
