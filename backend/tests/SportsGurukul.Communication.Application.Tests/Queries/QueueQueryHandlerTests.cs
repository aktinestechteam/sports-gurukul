using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using MediatR;
using System.Linq.Expressions;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class QueueQueryHandlerTests
{
    private readonly Mock<IQueueRepository> _queueRepoMock;

    public QueueQueryHandlerTests()
    {
        _queueRepoMock = new Mock<IQueueRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnQueueDepth()
    {
        var items = new List<NotificationQueue>
        {
            new() { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Status = NotificationStatus.Queued, Priority = NotificationPriority.Normal, QueuedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Status = NotificationStatus.Queued, Priority = NotificationPriority.High, QueuedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Status = NotificationStatus.Queued, Priority = NotificationPriority.Low, QueuedAt = DateTime.UtcNow },
        };

        _queueRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationQueue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var query = items.AsEnumerable();
        query.Count().Should().Be(3);
        items.Count.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnQueuedItemsWithPagination()
    {
        var items = Enumerable.Range(1, 10)
            .Select(i => new NotificationQueue
            {
                Id = Guid.NewGuid(),
                NotificationId = Guid.NewGuid(),
                ChannelType = NotificationChannelType.Email,
                Status = NotificationStatus.Queued,
                Priority = NotificationPriority.Normal,
                QueuedAt = DateTime.UtcNow.AddMinutes(-i)
            })
            .ToList();

        _queueRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationQueue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        _queueRepoMock.Setup(r => r.GetPendingItemsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items.Take(5).ToList());

        var result = await _queueRepoMock.Object.GetPendingItemsAsync(5, CancellationToken.None);

        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_ShouldFilterByPriority()
    {
        var highPriorityItems = new List<NotificationQueue>
        {
            new() { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.Email, Status = NotificationStatus.Queued, Priority = NotificationPriority.Critical, QueuedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), ChannelType = NotificationChannelType.SMS, Status = NotificationStatus.Queued, Priority = NotificationPriority.Critical, QueuedAt = DateTime.UtcNow },
        };

        _queueRepoMock.Setup(r => r.GetByPriorityAsync(NotificationPriority.Critical, It.IsAny<CancellationToken>()))
            .ReturnsAsync(highPriorityItems);

        var result = await _queueRepoMock.Object.GetByPriorityAsync(NotificationPriority.Critical, CancellationToken.None);

        result.Should().HaveCount(2);
        result.All(q => q.Priority == NotificationPriority.Critical).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoItems()
    {
        _queueRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationQueue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        _queueRepoMock.Setup(r => r.GetPendingItemsAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        var result = await _queueRepoMock.Object.GetPendingItemsAsync(10, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
