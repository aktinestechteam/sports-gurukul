using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Delivery;

public class PriorityQueueProcessorTests
{
    private readonly Mock<IQueueRepository> _queueRepo;
    private readonly Mock<INotificationRepository> _notificationRepo;
    private readonly Mock<INotificationDispatcher> _dispatcher;
    private readonly Mock<ILogger<PriorityQueueProcessor>> _logger;
    private readonly PriorityQueueProcessor _processor;

    public PriorityQueueProcessorTests()
    {
        _queueRepo = new Mock<IQueueRepository>();
        _notificationRepo = new Mock<INotificationRepository>();
        _dispatcher = new Mock<INotificationDispatcher>();
        _logger = new Mock<ILogger<PriorityQueueProcessor>>();
        _processor = new PriorityQueueProcessor(_queueRepo.Object, _notificationRepo.Object, _dispatcher.Object, _logger.Object);
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_ProcessesItems_InPriorityOrder()
    {
        var lowItem = TestDataFactory.CreateQueueItem(priority: NotificationPriority.Low);
        var highItem = TestDataFactory.CreateQueueItem(priority: NotificationPriority.High);
        var criticalItem = TestDataFactory.CreateQueueItem(priority: NotificationPriority.Critical);

        var pendingItems = new List<NotificationQueue> { lowItem, highItem, criticalItem };

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingItems);

        _dispatcher.Setup(d => d.DispatchAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        _dispatcher.Verify(d => d.DispatchAsync(criticalItem.NotificationId, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync(highItem.NotificationId, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync(lowItem.NotificationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_MarksItemAsSent_OnSuccess()
    {
        var item = TestDataFactory.CreateQueueItem();

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { item });

        _dispatcher.Setup(d => d.DispatchAsync(item.NotificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        item.Status.Should().Be(NotificationStatus.Sent);
        item.ProcessCompletedAt.Should().NotBeNull();
        item.LockToken.Should().BeNull();
        item.LockExpiresAt.Should().BeNull();
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_MarksItemAsFailed_OnDispatchFailure()
    {
        var item = TestDataFactory.CreateQueueItem();
        item.MaxAttempts = 1;

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { item });

        _dispatcher.Setup(d => d.DispatchAsync(item.NotificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Dispatch failed"));

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        item.Status.Should().Be(NotificationStatus.Failed);
        item.AttemptCount.Should().Be(1);
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_RequeuesItem_WhenUnderMaxAttempts()
    {
        var item = TestDataFactory.CreateQueueItem();
        item.MaxAttempts = 3;

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { item });

        _dispatcher.Setup(d => d.DispatchAsync(item.NotificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Temporary failure"));

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        item.Status.Should().Be(NotificationStatus.Queued);
        item.AttemptCount.Should().Be(1);
        item.NextAttemptAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_HandlesException_Gracefully()
    {
        var item = TestDataFactory.CreateQueueItem();

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { item });

        _dispatcher.Setup(d => d.DispatchAsync(item.NotificationId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        item.Status.Should().Be(NotificationStatus.Failed);
        _queueRepo.Verify(r => r.Update(item), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_SetsLockToken_WhenProcessing()
    {
        var item = TestDataFactory.CreateQueueItem();

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { item });

        _dispatcher.Setup(d => d.DispatchAsync(item.NotificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        item.LockToken.Should().BeNull();
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_StopsProcessing_WhenCancellationRequested()
    {
        var items = new List<NotificationQueue>
        {
            TestDataFactory.CreateQueueItem(),
            TestDataFactory.CreateQueueItem()
        };

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        _dispatcher.Setup(d => d.DispatchAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await _processor.ProcessQueueItemsAsync(cts.Token);

        _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_ReturnsNothing_WhenQueueEmpty()
    {
        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessQueueItemsAsync_ProcessesItems_ByQueuedAt_WhenSamePriority()
    {
        var olderItem = TestDataFactory.CreateQueueItem(priority: NotificationPriority.Normal);
        olderItem.QueuedAt = DateTime.UtcNow.AddMinutes(-10);

        var newerItem = TestDataFactory.CreateQueueItem(priority: NotificationPriority.Normal);
        newerItem.QueuedAt = DateTime.UtcNow;

        _queueRepo.Setup(r => r.GetPendingItemsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { newerItem, olderItem });

        _dispatcher.Setup(d => d.DispatchAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        await _processor.ProcessQueueItemsAsync(CancellationToken.None);

        _dispatcher.Verify(d => d.DispatchAsync(olderItem.NotificationId, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync(newerItem.NotificationId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
