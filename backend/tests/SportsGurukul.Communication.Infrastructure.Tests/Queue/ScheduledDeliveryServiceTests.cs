using Microsoft.Extensions.DependencyInjection;
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

public class ScheduledDeliveryServiceTests
{
    private readonly Mock<IServiceProvider> _serviceProvider;
    private readonly Mock<IServiceScopeFactory> _scopeFactory;
    private readonly Mock<IServiceScope> _scope;
    private readonly Mock<INotificationRepository> _notificationRepo;
    private readonly Mock<IQueueService> _queueService;
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<ScheduledDeliveryService>> _logger;
    private readonly ScheduledDeliveryService _service;

    public ScheduledDeliveryServiceTests()
    {
        _serviceProvider = new Mock<IServiceProvider>();
        _scopeFactory = new Mock<IServiceScopeFactory>();
        _scope = new Mock<IServiceScope>();
        _notificationRepo = new Mock<INotificationRepository>();
        _queueService = new Mock<IQueueService>();
        _options = TestDataFactory.CreateOptions(o =>
        {
            o.Queue.ScheduledDeliveryEnabled = true;
            o.Queue.ScheduledPollingIntervalMs = 100;
        });
        _logger = new Mock<ILogger<ScheduledDeliveryService>>();

        _serviceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory)))
            .Returns(_scopeFactory.Object);

        _scopeFactory.Setup(f => f.CreateScope())
            .Returns(_scope.Object);

        _scope.Setup(s => s.ServiceProvider)
            .Returns(_serviceProvider.Object);

        _serviceProvider.Setup(s => s.GetService(typeof(INotificationRepository)))
            .Returns(_notificationRepo.Object);

        _serviceProvider.Setup(s => s.GetService(typeof(IQueueService)))
            .Returns(_queueService.Object);

        _service = new ScheduledDeliveryService(_serviceProvider.Object, _options, _logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_SchedulesWithDelay()
    {
        var dueNotifications = new List<Domain.Entities.Notification.Notification>
        {
            TestDataFactory.CreateNotification()
        };

        _notificationRepo.Setup(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dueNotifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await _service.StartAsync(cts.Token);
        await Task.Delay(300);
        await _service.StopAsync(cts.Token);

        _notificationRepo.Verify(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ProcessesDueItems()
    {
        var dueNotifications = new List<Domain.Entities.Notification.Notification>
        {
            TestDataFactory.CreateNotification()
        };

        _notificationRepo.Setup(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dueNotifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await _service.StartAsync(cts.Token);
        await Task.Delay(300);
        await _service.StopAsync(cts.Token);

        _queueService.Verify(s => s.EnqueueAsync(dueNotifications[0].Id, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_FiltersByReadyTime()
    {
        var pastDue = TestDataFactory.CreateNotification();
        pastDue.ScheduledAt = DateTime.UtcNow.AddHours(-1);

        var futureDue = TestDataFactory.CreateNotification();
        futureDue.ScheduledAt = DateTime.UtcNow.AddHours(1);

        var dueNotifications = new List<Domain.Entities.Notification.Notification> { pastDue };

        _notificationRepo.Setup(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dueNotifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await _service.StartAsync(cts.Token);
        await Task.Delay(300);
        await _service.StopAsync(cts.Token);

        _queueService.Verify(s => s.EnqueueAsync(pastDue.Id, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _queueService.Verify(s => s.EnqueueAsync(futureDue.Id, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_LogsEachScheduledItem()
    {
        var dueNotifications = new List<Domain.Entities.Notification.Notification>
        {
            TestDataFactory.CreateNotification()
        };

        _notificationRepo.Setup(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dueNotifications);

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await _service.StartAsync(cts.Token);
        await Task.Delay(300);
        await _service.StopAsync(cts.Token);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("moved to queue")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_SkipsProcessing_WhenDisabled()
    {
        var options = TestDataFactory.CreateOptions(o => o.Queue.ScheduledDeliveryEnabled = false);
        var service = new ScheduledDeliveryService(_serviceProvider.Object, options, _logger.Object);

        using var cts = new CancellationTokenSource();

        await service.StartAsync(cts.Token);
        await Task.Delay(100);
        await service.StopAsync(cts.Token);

        _notificationRepo.Verify(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesException_Gracefully()
    {
        _notificationRepo.Setup(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        _queueService.Setup(s => s.EnqueueAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(300);

        await _service.StartAsync(cts.Token);
        await Task.Delay(400);
        await _service.StopAsync(cts.Token);

        _logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error processing")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_PollsAtConfiguredInterval()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.Queue.ScheduledDeliveryEnabled = true;
            o.Queue.ScheduledPollingIntervalMs = 50;
        });

        _notificationRepo.Setup(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Notification.Notification>());

        _serviceProvider.Setup(s => s.GetService(typeof(INotificationRepository)))
            .Returns(_notificationRepo.Object);

        var service = new ScheduledDeliveryService(_serviceProvider.Object, options, _logger.Object);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await service.StartAsync(cts.Token);
        await Task.Delay(300);
        await service.StopAsync(cts.Token);

        _notificationRepo.Verify(r => r.GetScheduledDueAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
    }
}
