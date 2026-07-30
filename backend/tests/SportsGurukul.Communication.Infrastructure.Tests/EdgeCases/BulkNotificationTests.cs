using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Queue;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class BulkNotificationTests
{
    private readonly Mock<INotificationRepository> _notifRepoMock = new();
    private readonly Mock<IQueueService> _queueServiceMock = new();
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<BulkDeliveryService>> _loggerMock = new();

    public BulkNotificationTests()
    {
        _options = Options.Create(new CommunicationOptions
        {
            Delivery = new DeliveryOptions
            {
                BulkBatchSize = 100,
                ThrottleDelayMs = 10
            }
        });
    }

    [Fact]
    public async Task ProcessBulkAsync_ProcessesAllRecipients()
    {
        var batchId = Guid.NewGuid();
        var notifications = Enumerable.Range(0, 5).Select(i => new Notification
        {
            Id = Guid.NewGuid(),
            Priority = NotificationPriority.Normal
        }).ToList();

        _notifRepoMock
            .Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueServiceMock
            .Setup(q => q.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var service = new BulkDeliveryService(
            _notifRepoMock.Object, _queueServiceMock.Object, _options, _loggerMock.Object);

        var result = await service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.SuccessCount.Should().Be(5);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public async Task ProcessBulkAsync_EmptyRecipientList_ReturnsZeroCounts()
    {
        var batchId = Guid.NewGuid();
        _notifRepoMock
            .Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var service = new BulkDeliveryService(
            _notifRepoMock.Object, _queueServiceMock.Object, _options, _loggerMock.Object);

        var result = await service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public async Task ProcessBulkAsync_ReportsSuccessAndFailureCounts()
    {
        var batchId = Guid.NewGuid();
        var notifications = Enumerable.Range(0, 10).Select(i => new Notification
        {
            Id = Guid.NewGuid(),
            Priority = NotificationPriority.Normal
        }).ToList();

        _notifRepoMock
            .Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueServiceMock
            .Setup(q => q.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var service = new BulkDeliveryService(
            _notifRepoMock.Object, _queueServiceMock.Object, _options, _loggerMock.Object);

        var result = await service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.SuccessCount.Should().Be(10);
        result.TotalCount.Should().Be(10);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public async Task ProcessBulkAsync_RespectsPriorityOrder()
    {
        var batchId = Guid.NewGuid();
        var high = new Notification { Id = Guid.NewGuid(), Priority = NotificationPriority.High };
        var low = new Notification { Id = Guid.NewGuid(), Priority = NotificationPriority.Low };
        var normal = new Notification { Id = Guid.NewGuid(), Priority = NotificationPriority.Normal };

        _notifRepoMock
            .Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification> { low, normal, high });

        var enqueuedIds = new List<Guid>();
        _queueServiceMock
            .Setup(q => q.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Callback<Guid, CancellationToken>((id, _) => enqueuedIds.Add(id))
            .ReturnsAsync(Result<bool>.Success(true));

        var service = new BulkDeliveryService(
            _notifRepoMock.Object, _queueServiceMock.Object, _options, _loggerMock.Object);

        await service.ProcessBulkAsync(batchId, CancellationToken.None);

        enqueuedIds[0].Should().Be(high.Id);
    }

    [Fact]
    public async Task ProcessBulkAsync_PartialEnqueueFailure_StillCompletes()
    {
        var batchId = Guid.NewGuid();
        var notifications = Enumerable.Range(0, 3).Select(i => new Notification
        {
            Id = Guid.NewGuid(),
            Priority = NotificationPriority.Normal
        }).ToList();

        _notifRepoMock
            .Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var callCount = 0;
        _queueServiceMock
            .Setup(q => q.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                callCount++;
                if (callCount == 2)
                    return Task.FromResult(Result<bool>.Failure("Queue full"));
                return Task.FromResult(Result<bool>.Success(true));
            });

        var service = new BulkDeliveryService(
            _notifRepoMock.Object, _queueServiceMock.Object, _options, _loggerMock.Object);

        var result = await service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public async Task ProcessBulkAsync_WithBulkBatchSize_ProcessesInChunks()
    {
        var batchId = Guid.NewGuid();
        var options = Options.Create(new CommunicationOptions
        {
            Delivery = new DeliveryOptions
            {
                BulkBatchSize = 2,
                ThrottleDelayMs = 5
            }
        });
        var notifications = Enumerable.Range(0, 5).Select(i => new Notification
        {
            Id = Guid.NewGuid(),
            Priority = NotificationPriority.Normal
        }).ToList();

        _notifRepoMock
            .Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var enqueueCount = 0;
        _queueServiceMock
            .Setup(q => q.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Callback(() => enqueueCount++)
            .ReturnsAsync(Result<bool>.Success(true));

        var service = new BulkDeliveryService(
            _notifRepoMock.Object, _queueServiceMock.Object, options, _loggerMock.Object);

        await service.ProcessBulkAsync(batchId, CancellationToken.None);

        enqueueCount.Should().Be(5);
    }
}
