using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class DeadLetterQueueTests
{
    private readonly Mock<IQueueRepository> _queueRepoMock = new();
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<DeadLetterQueueHandler>> _loggerMock = new();

    public DeadLetterQueueTests()
    {
        _options = Options.Create(new CommunicationOptions
        {
            Delivery = new DeliveryOptions
            {
                DeadLetterEnabled = true
            }
        });
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_MovesStaleItemsToFailed()
    {
        var staleItem = new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = Guid.NewGuid(),
            Status = NotificationStatus.Sending,
            Priority = NotificationPriority.Normal
        };
        _queueRepoMock
            .Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { staleItem });

        var handler = new DeadLetterQueueHandler(
            _queueRepoMock.Object, _options, _loggerMock.Object);

        await handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        staleItem.Status.Should().Be(NotificationStatus.Failed);
        _queueRepoMock.Verify(r => r.Update(staleItem), Times.Once);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_WhenDisabled_DoesNothing()
    {
        var options = Options.Create(new CommunicationOptions
        {
            Delivery = new DeliveryOptions
            {
                DeadLetterEnabled = false
            }
        });

        var handler = new DeadLetterQueueHandler(
            _queueRepoMock.Object, options, _loggerMock.Object);

        await handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        _queueRepoMock.Verify(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_NoStaleItems_DoesNothing()
    {
        _queueRepoMock
            .Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        var handler = new DeadLetterQueueHandler(
            _queueRepoMock.Object, _options, _loggerMock.Object);

        await handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        _queueRepoMock.Verify(r => r.Update(It.IsAny<NotificationQueue>()), Times.Never);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_RepositoryException_HandlesGracefully()
    {
        _queueRepoMock
            .Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var handler = new DeadLetterQueueHandler(
            _queueRepoMock.Object, _options, _loggerMock.Object);

        var act = () => handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_MultipleStaleItems_AllMarkedFailed()
    {
        var items = Enumerable.Range(0, 5).Select(i => new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = Guid.NewGuid(),
            Status = NotificationStatus.Queued,
            Priority = NotificationPriority.Normal
        }).ToList();

        _queueRepoMock
            .Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var handler = new DeadLetterQueueHandler(
            _queueRepoMock.Object, _options, _loggerMock.Object);

        await handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        items.Should().AllSatisfy(i => i.Status.Should().Be(NotificationStatus.Failed));
        _queueRepoMock.Verify(r => r.Update(It.IsAny<NotificationQueue>()), Times.Exactly(5));
    }
}
