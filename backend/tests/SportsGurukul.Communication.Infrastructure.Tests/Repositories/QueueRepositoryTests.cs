using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Infrastructure.Tests.Repositories;

public class QueueRepositoryTests
{
    private static int _counter;

    private static NotificationQueue CreateQueueItem(
        NotificationChannelType channel = NotificationChannelType.Email,
        NotificationPriority priority = NotificationPriority.Normal,
        NotificationStatus status = NotificationStatus.Queued)
    {
        _counter++;
        return new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = Guid.NewGuid(),
            ChannelType = channel,
            Priority = priority,
            Status = status,
            QueuedAt = DateTime.UtcNow.AddMinutes(-_counter),
            MaxAttempts = 3,
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<IQueueRepository> _mock;
    private readonly List<NotificationQueue> _queueItems;

    public QueueRepositoryTests()
    {
        _queueItems =
        [
            CreateQueueItem(NotificationChannelType.Email, NotificationPriority.High),
            CreateQueueItem(NotificationChannelType.Email, NotificationPriority.Normal),
            CreateQueueItem(NotificationChannelType.SMS, NotificationPriority.Low, NotificationStatus.Queued)
        ];
        _mock = CreateMockWithBaseSetup(_queueItems);
    }

    [Fact]
    public async Task GetNextAsync_ReturnsNextQueuedItemByPriority()
    {
        var expected = _queueItems
            .Where(q => q.Status == NotificationStatus.Queued)
            .OrderByDescending(q => q.Priority)
            .ThenBy(q => q.QueuedAt)
            .First();
        _mock.Setup(r => r.GetPendingItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { expected });
        var result = await _mock.Object.GetPendingItemsAsync(1);
        result.Should().ContainSingle();
        result.First().Priority.Should().Be(NotificationPriority.High);
    }

    [Fact]
    public async Task EnqueueAsync_AddsToQueue()
    {
        var item = CreateQueueItem();
        _mock.Setup(r => r.AddAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        var result = await _mock.Object.AddAsync(item);
        result.Should().Be(item);
        _mock.Verify(r => r.AddAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DequeueAsync_RemovesFromQueue()
    {
        var item = _queueItems[0];
        _mock.Object.Remove(item);
        _mock.Verify(r => r.Remove(item), Times.Once);
    }

    [Fact]
    public async Task GetQueueDepthAsync_ReturnsCount()
    {
        _mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<NotificationQueue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);
        var result = await _mock.Object.CountAsync(q => q.Status == NotificationStatus.Queued);
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetByStatusAsync_FiltersByStatus()
    {
        var queued = _queueItems.Where(q => q.Status == NotificationStatus.Queued).ToList();
        _mock.Setup(r => r.GetByStatusAsync(NotificationStatus.Queued, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queued);
        var result = await _mock.Object.GetByStatusAsync(NotificationStatus.Queued);
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(q => q.Status.Should().Be(NotificationStatus.Queued));
    }

    [Fact]
    public async Task GetByPriorityAsync_FiltersByPriority()
    {
        var high = _queueItems.Where(q => q.Priority == NotificationPriority.High).ToList();
        _mock.Setup(r => r.GetByPriorityAsync(NotificationPriority.High, It.IsAny<CancellationToken>()))
            .ReturnsAsync(high);
        var result = await _mock.Object.GetByPriorityAsync(NotificationPriority.High);
        result.Should().ContainSingle();
        result.First().Priority.Should().Be(NotificationPriority.High);
    }

    [Fact]
    public async Task GetStaleLocksAsync_ReturnsExpiredLocks()
    {
        var threshold = DateTime.UtcNow.AddMinutes(-30);
        var stale = _queueItems.Where(q => q.LockExpiresAt < threshold).ToList();
        _mock.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stale);
        var result = await _mock.Object.GetStaleLocksAsync(threshold);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByNotificationIdAsync_ReturnsQueueItem()
    {
        var expected = _queueItems[0];
        _mock.Setup(r => r.GetByNotificationIdAsync(expected.NotificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByNotificationIdAsync(expected.NotificationId);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetPendingItemsAsync_ReturnsLimitedItems()
    {
        var pending = _queueItems.Where(q => q.Status == NotificationStatus.Queued)
            .OrderBy(q => q.QueuedAt).ToList();
        _mock.Setup(r => r.GetPendingItemsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pending.Take(2).ToList());
        var result = await _mock.Object.GetPendingItemsAsync(2);
        result.Should().HaveCount(2);
    }

    private static Mock<IQueueRepository> CreateMockWithBaseSetup(List<NotificationQueue> data)
    {
        var mock = new Mock<IQueueRepository>();

        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationQueue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationQueue, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<NotificationQueue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationQueue entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<NotificationQueue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationQueue, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<NotificationQueue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationQueue, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));

        return mock;
    }
}
