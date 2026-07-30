using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Delivery;

public class DeadLetterQueueHandlerTests
{
    private readonly Mock<IQueueRepository> _queueRepo;
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<DeadLetterQueueHandler>> _logger;
    private readonly DeadLetterQueueHandler _handler;

    public DeadLetterQueueHandlerTests()
    {
        _queueRepo = new Mock<IQueueRepository>();
        _options = TestDataFactory.CreateOptions(o => o.Delivery.DeadLetterEnabled = true);
        _logger = new Mock<ILogger<DeadLetterQueueHandler>>();
        _handler = new DeadLetterQueueHandler(_queueRepo.Object, _options, _logger.Object);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_MovesStaleItems_ToFailedStatus()
    {
        var staleItem = new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = Guid.NewGuid(),
            Status = NotificationStatus.Queued,
            Priority = NotificationPriority.Normal
        };

        _queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { staleItem });

        await _handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        staleItem.Status.Should().Be(NotificationStatus.Failed);
        _queueRepo.Verify(r => r.Update(staleItem), Times.Once);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_DoesNothing_WhenDisabled()
    {
        var options = TestDataFactory.CreateOptions(o => o.Delivery.DeadLetterEnabled = false);
        var handler = new DeadLetterQueueHandler(_queueRepo.Object, options, _logger.Object);

        await handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        _queueRepo.Verify(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_ProcessesMultipleStaleItems()
    {
        var staleItems = new List<NotificationQueue>
        {
            new NotificationQueue { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), Status = NotificationStatus.Queued },
            new NotificationQueue { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), Status = NotificationStatus.Sending },
            new NotificationQueue { Id = Guid.NewGuid(), NotificationId = Guid.NewGuid(), Status = NotificationStatus.Queued }
        };

        _queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(staleItems);

        await _handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        staleItems.All(i => i.Status == NotificationStatus.Failed).Should().BeTrue();
        _queueRepo.Verify(r => r.Update(It.IsAny<NotificationQueue>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_HandlesEmptyList()
    {
        _queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        await _handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        _queueRepo.Verify(r => r.Update(It.IsAny<NotificationQueue>()), Times.Never);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_PassesCorrectThreshold()
    {
        _queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        await _handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        _queueRepo.Verify(r => r.GetStaleLocksAsync(
            It.Is<DateTime>(d => d <= DateTime.UtcNow.AddMinutes(-29) && d >= DateTime.UtcNow.AddMinutes(-31)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessDeadLetterQueueAsync_LogsWarning_ForEachStaleItem()
    {
        var staleItem = new NotificationQueue
        {
            Id = Guid.NewGuid(),
            NotificationId = Guid.NewGuid(),
            Status = NotificationStatus.Queued
        };

        _queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue> { staleItem });

        await _handler.ProcessDeadLetterQueueAsync(CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Moving stale queue item")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
