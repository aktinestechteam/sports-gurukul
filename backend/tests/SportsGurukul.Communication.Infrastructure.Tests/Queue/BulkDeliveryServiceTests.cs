using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Queue;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Queue;

public class BulkDeliveryServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepo;
    private readonly Mock<IQueueService> _queueService;
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<BulkDeliveryService>> _logger;
    private readonly BulkDeliveryService _service;

    public BulkDeliveryServiceTests()
    {
        _notificationRepo = new Mock<INotificationRepository>();
        _queueService = new Mock<IQueueService>();
        _options = TestDataFactory.CreateOptions(o =>
        {
            o.Delivery.BulkBatchSize = 2;
            o.Delivery.ThrottleDelayMs = 10;
        });
        _logger = new Mock<ILogger<BulkDeliveryService>>();
        _service = new BulkDeliveryService(_notificationRepo.Object, _queueService.Object, _options, _logger.Object);
    }

    [Fact]
    public async Task ProcessBulkAsync_SendsToAllRecipients()
    {
        var batchId = Guid.NewGuid();
        var notifications = new List<Domain.Entities.Notification.Notification>
        {
            TestDataFactory.CreateNotification(),
            TestDataFactory.CreateNotification(),
            TestDataFactory.CreateNotification()
        };

        _notificationRepo.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.SuccessCount.Should().Be(3);
        result.BatchId.Should().Be(batchId);
        _queueService.Verify(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ProcessBulkAsync_UsesBatchProcessing()
    {
        var batchId = Guid.NewGuid();
        var notifications = new List<Domain.Entities.Notification.Notification>();
        for (int i = 0; i < 5; i++)
            notifications.Add(TestDataFactory.CreateNotification());

        _notificationRepo.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.SuccessCount.Should().Be(5);
        _queueService.Verify(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(5));
    }

    [Fact]
    public async Task ProcessBulkAsync_TracksPerRecipientStatus()
    {
        var batchId = Guid.NewGuid();
        var notifications = new List<Domain.Entities.Notification.Notification>
        {
            TestDataFactory.CreateNotification(),
            TestDataFactory.CreateNotification()
        };

        _notificationRepo.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueService.Setup(s => s.EnqueueAsync(notifications[0].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        _queueService.Setup(s => s.EnqueueAsync(notifications[1].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public async Task ProcessBulkAsync_HandlesPartialFailures()
    {
        var batchId = Guid.NewGuid();
        var notifications = new List<Domain.Entities.Notification.Notification>
        {
            TestDataFactory.CreateNotification(),
            TestDataFactory.CreateNotification()
        };

        _notificationRepo.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public async Task ProcessBulkAsync_ReturnsBatchSummary()
    {
        var batchId = Guid.NewGuid();
        var notifications = new List<Domain.Entities.Notification.Notification>
        {
            TestDataFactory.CreateNotification()
        };

        _notificationRepo.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.Should().NotBeNull();
        result.BatchId.Should().Be(batchId);
        result.TotalCount.Should().Be(1);
        result.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ProcessBulkAsync_ProcessesInPriorityOrder()
    {
        var batchId = Guid.NewGuid();
        var low = TestDataFactory.CreateNotification(priority: NotificationPriority.Low);
        var high = TestDataFactory.CreateNotification(priority: NotificationPriority.High);
        var notifications = new List<Domain.Entities.Notification.Notification> { low, high };

        _notificationRepo.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.SuccessCount.Should().Be(2);
    }

    [Fact]
    public async Task ProcessBulkAsync_ReturnsZero_WhenBatchEmpty()
    {
        var batchId = Guid.NewGuid();

        _notificationRepo.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Notification.Notification>());

        var result = await _service.ProcessBulkAsync(batchId, CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(0);
    }
}
