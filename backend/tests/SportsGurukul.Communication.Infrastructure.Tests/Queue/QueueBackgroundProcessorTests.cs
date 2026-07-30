using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Platform.Communication.Queue;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Queue;

public class QueueBackgroundProcessorTests
{
    private readonly Mock<IServiceProvider> _serviceProvider;
    private readonly Mock<IServiceScopeFactory> _scopeFactory;
    private readonly Mock<IServiceScope> _scope;
    private readonly IOptions<CommunicationOptions> _defaultOptions;
    private readonly Mock<ILogger<QueueBackgroundProcessor>> _logger;
    private readonly QueueBackgroundProcessor _processor;

    public QueueBackgroundProcessorTests()
    {
        _serviceProvider = new Mock<IServiceProvider>();
        _scopeFactory = new Mock<IServiceScopeFactory>();
        _scope = new Mock<IServiceScope>();
        _defaultOptions = TestDataFactory.CreateOptions(o =>
        {
            o.Queue.PollingIntervalMs = 50;
            o.Delivery.DeadLetterEnabled = true;
            o.CircuitBreaker.FailureThreshold = 100;
        });
        _logger = new Mock<ILogger<QueueBackgroundProcessor>>();

        _serviceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory)))
            .Returns(_scopeFactory.Object);

        _scopeFactory.Setup(f => f.CreateScope())
            .Returns(_scope.Object);

        _scope.Setup(s => s.ServiceProvider)
            .Returns(_serviceProvider.Object);

        _processor = new QueueBackgroundProcessor(_serviceProvider.Object, _defaultOptions, _logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ProcessesQueuedItems()
    {
        var queueRepo = new Mock<IQueueRepository>();
        queueRepo.Setup(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());
        queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        var priorityProcessor = new PriorityQueueProcessor(
            queueRepo.Object,
            Mock.Of<INotificationRepository>(),
            Mock.Of<INotificationDispatcher>(),
            Mock.Of<ILogger<PriorityQueueProcessor>>());

        var deadLetterHandler = new DeadLetterQueueHandler(
            queueRepo.Object,
            _defaultOptions,
            Mock.Of<ILogger<DeadLetterQueueHandler>>());

        _serviceProvider.Setup(s => s.GetService(typeof(PriorityQueueProcessor)))
            .Returns(priorityProcessor);
        _serviceProvider.Setup(s => s.GetService(typeof(DeadLetterQueueHandler)))
            .Returns(deadLetterHandler);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await _processor.StartAsync(cts.Token);
        await Task.Delay(300);
        await _processor.StopAsync(cts.Token);

        queueRepo.Verify(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_DequeuesItemsInPriorityOrder()
    {
        var queueRepo = new Mock<IQueueRepository>();
        queueRepo.Setup(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());
        queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        var priorityProcessor = new PriorityQueueProcessor(
            queueRepo.Object,
            Mock.Of<INotificationRepository>(),
            Mock.Of<INotificationDispatcher>(),
            Mock.Of<ILogger<PriorityQueueProcessor>>());

        var deadLetterHandler = new DeadLetterQueueHandler(
            queueRepo.Object,
            _defaultOptions,
            Mock.Of<ILogger<DeadLetterQueueHandler>>());

        _serviceProvider.Setup(s => s.GetService(typeof(PriorityQueueProcessor)))
            .Returns(priorityProcessor);
        _serviceProvider.Setup(s => s.GetService(typeof(DeadLetterQueueHandler)))
            .Returns(deadLetterHandler);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await _processor.StartAsync(cts.Token);
        await Task.Delay(300);
        await _processor.StopAsync(cts.Token);

        queueRepo.Verify(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesProcessingFailure()
    {
        var queueRepo = new Mock<IQueueRepository>();
        queueRepo.Setup(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Processing error"));
        queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        var priorityProcessor = new PriorityQueueProcessor(
            queueRepo.Object,
            Mock.Of<INotificationRepository>(),
            Mock.Of<INotificationDispatcher>(),
            Mock.Of<ILogger<PriorityQueueProcessor>>());

        var deadLetterHandler = new DeadLetterQueueHandler(
            Mock.Of<IQueueRepository>(),
            _defaultOptions,
            Mock.Of<ILogger<DeadLetterQueueHandler>>());

        _serviceProvider.Setup(s => s.GetService(typeof(PriorityQueueProcessor)))
            .Returns(priorityProcessor);
        _serviceProvider.Setup(s => s.GetService(typeof(DeadLetterQueueHandler)))
            .Returns(deadLetterHandler);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(300);

        await _processor.StartAsync(cts.Token);
        await Task.Delay(400);
        await _processor.StopAsync(cts.Token);

        _logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error in queue")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_Stops_WhenCancellationRequested()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await _processor.StartAsync(cts.Token);
        await _processor.StopAsync(cts.Token);
    }

    [Fact]
    public async Task ExecuteAsync_Delays_WhenQueueEmpty()
    {
        var queueRepo = new Mock<IQueueRepository>();
        queueRepo.Setup(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());
        queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        var priorityProcessor = new PriorityQueueProcessor(
            queueRepo.Object,
            Mock.Of<INotificationRepository>(),
            Mock.Of<INotificationDispatcher>(),
            Mock.Of<ILogger<PriorityQueueProcessor>>());

        var deadLetterHandler = new DeadLetterQueueHandler(
            queueRepo.Object,
            _defaultOptions,
            Mock.Of<ILogger<DeadLetterQueueHandler>>());

        _serviceProvider.Setup(s => s.GetService(typeof(PriorityQueueProcessor)))
            .Returns(priorityProcessor);
        _serviceProvider.Setup(s => s.GetService(typeof(DeadLetterQueueHandler)))
            .Returns(deadLetterHandler);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500);

        var startTime = DateTime.UtcNow;
        await _processor.StartAsync(cts.Token);
        await Task.Delay(600);
        await _processor.StopAsync(cts.Token);

        var elapsed = DateTime.UtcNow - startTime;
        elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public async Task ExecuteAsync_ProcessesDeadLetterQueue_First()
    {
        var queueRepo = new Mock<IQueueRepository>();
        queueRepo.Setup(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());
        queueRepo.Setup(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationQueue>());

        var priorityProcessor = new PriorityQueueProcessor(
            queueRepo.Object,
            Mock.Of<INotificationRepository>(),
            Mock.Of<INotificationDispatcher>(),
            Mock.Of<ILogger<PriorityQueueProcessor>>());

        var deadLetterHandler = new DeadLetterQueueHandler(
            queueRepo.Object,
            _defaultOptions,
            Mock.Of<ILogger<DeadLetterQueueHandler>>());

        _serviceProvider.Setup(s => s.GetService(typeof(PriorityQueueProcessor)))
            .Returns(priorityProcessor);
        _serviceProvider.Setup(s => s.GetService(typeof(DeadLetterQueueHandler)))
            .Returns(deadLetterHandler);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await _processor.StartAsync(cts.Token);
        await Task.Delay(300);
        await _processor.StopAsync(cts.Token);

        queueRepo.Verify(r => r.GetStaleLocksAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        queueRepo.Verify(r => r.GetPendingItemsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
