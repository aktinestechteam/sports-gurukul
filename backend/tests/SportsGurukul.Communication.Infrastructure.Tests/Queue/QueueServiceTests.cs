using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Queue;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Queue;

public class QueueServiceTests
{
    private readonly Mock<IQueueRepository> _queueRepo;
    private readonly Mock<INotificationRepository> _notificationRepo;
    private readonly Mock<IDeliveryRepository> _deliveryRepo;
    private readonly Mock<ILogger<QueueService>> _logger;
    private readonly QueueService _service;

    public QueueServiceTests()
    {
        _queueRepo = new Mock<IQueueRepository>();
        _notificationRepo = new Mock<INotificationRepository>();
        _deliveryRepo = new Mock<IDeliveryRepository>();
        _logger = new Mock<ILogger<QueueService>>();
        _service = new QueueService(_queueRepo.Object, _notificationRepo.Object, _deliveryRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task EnqueueAsync_AddsItemToQueue()
    {
        var notification = TestDataFactory.CreateNotification();

        _notificationRepo.Setup(r => r.GetByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _queueRepo.Setup(r => r.GetByNotificationIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationQueue?)null);

        _queueRepo.Setup(r => r.AddAsync(It.IsAny<NotificationQueue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationQueue q, CancellationToken _) => q);

        var result = await _service.EnqueueAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
        _queueRepo.Verify(r => r.AddAsync(It.IsAny<NotificationQueue>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EnqueueAsync_AssignsPriority()
    {
        var notification = TestDataFactory.CreateNotification(priority: NotificationPriority.High);

        _notificationRepo.Setup(r => r.GetByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _queueRepo.Setup(r => r.GetByNotificationIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationQueue?)null);

        _queueRepo.Setup(r => r.AddAsync(It.IsAny<NotificationQueue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationQueue q, CancellationToken _) => q);

        await _service.EnqueueAsync(notification.Id);

        _queueRepo.Verify(r => r.AddAsync(
            It.Is<NotificationQueue>(q => q.Priority == NotificationPriority.High),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EnqueueAsync_ReturnsFailure_WhenNotificationNotFound()
    {
        var notificationId = Guid.NewGuid();

        _notificationRepo.Setup(r => r.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Notification.Notification?)null);

        var result = await _service.EnqueueAsync(notificationId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task EnqueueAsync_ReturnsFailure_WhenAlreadyQueued()
    {
        var notification = TestDataFactory.CreateNotification();
        var existingQueueItem = TestDataFactory.CreateQueueItem(notificationId: notification.Id);

        _notificationRepo.Setup(r => r.GetByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _queueRepo.Setup(r => r.GetByNotificationIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingQueueItem);

        var result = await _service.EnqueueAsync(notification.Id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already queued");
    }

    [Fact]
    public async Task DequeueAsync_MarksItemAsCancelled()
    {
        var notification = TestDataFactory.CreateNotification();
        var queueItem = TestDataFactory.CreateQueueItem(notificationId: notification.Id);

        _queueRepo.Setup(r => r.GetByNotificationIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queueItem);

        var result = await _service.DequeueAsync(notification.Id);

        result.IsSuccess.Should().BeTrue();
        queueItem.Status.Should().Be(NotificationStatus.Cancelled);
        queueItem.ProcessCompletedAt.Should().NotBeNull();
        _queueRepo.Verify(r => r.Update(queueItem), Times.Once);
    }

    [Fact]
    public async Task DequeueAsync_ReturnsFailure_WhenNotInQueue()
    {
        var notificationId = Guid.NewGuid();

        _queueRepo.Setup(r => r.GetByNotificationIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationQueue?)null);

        var result = await _service.DequeueAsync(notificationId);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task MarkProcessingAsync_SetsSendingStatus()
    {
        var queueItem = TestDataFactory.CreateQueueItem();

        _queueRepo.Setup(r => r.GetByIdAsync(queueItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queueItem);

        var lockToken = Guid.NewGuid().ToString("N");
        var result = await _service.MarkProcessingAsync(queueItem.Id, lockToken);

        result.IsSuccess.Should().BeTrue();
        queueItem.Status.Should().Be(NotificationStatus.Sending);
        queueItem.LockToken.Should().Be(lockToken);
        queueItem.LockExpiresAt.Should().NotBeNull();
    }

    [Fact]
    public async Task MarkCompletedAsync_SetsSentStatus()
    {
        var queueItem = TestDataFactory.CreateQueueItem(status: NotificationStatus.Sending);

        _queueRepo.Setup(r => r.GetByIdAsync(queueItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queueItem);

        var result = await _service.MarkCompletedAsync(queueItem.Id);

        result.IsSuccess.Should().BeTrue();
        queueItem.Status.Should().Be(NotificationStatus.Sent);
        queueItem.LockToken.Should().BeNull();
        queueItem.LockExpiresAt.Should().BeNull();
    }

    [Fact]
    public async Task MarkFailedAsync_IncrementsAttemptCount()
    {
        var queueItem = TestDataFactory.CreateQueueItem();
        queueItem.MaxAttempts = 3;

        _queueRepo.Setup(r => r.GetByIdAsync(queueItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queueItem);

        var result = await _service.MarkFailedAsync(queueItem.Id);

        result.IsSuccess.Should().BeTrue();
        queueItem.AttemptCount.Should().Be(1);
        queueItem.Status.Should().Be(NotificationStatus.Queued);
    }

    [Fact]
    public async Task MarkFailedAsync_MarksAsFailed_WhenMaxAttemptsReached()
    {
        var queueItem = TestDataFactory.CreateQueueItem();
        queueItem.MaxAttempts = 1;

        _queueRepo.Setup(r => r.GetByIdAsync(queueItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queueItem);

        var result = await _service.MarkFailedAsync(queueItem.Id);

        result.IsSuccess.Should().BeTrue();
        queueItem.AttemptCount.Should().Be(1);
        queueItem.Status.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public async Task MarkFailedAsync_SetsNextAttemptAt_WhenRequeued()
    {
        var queueItem = TestDataFactory.CreateQueueItem();
        queueItem.MaxAttempts = 3;

        _queueRepo.Setup(r => r.GetByIdAsync(queueItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queueItem);

        await _service.MarkFailedAsync(queueItem.Id);

        queueItem.NextAttemptAt.Should().NotBeNull();
    }
}
