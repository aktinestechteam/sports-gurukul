using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Queue;

public class ScheduledDeliveryService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly QueueOptions _options;
    private readonly ILogger<ScheduledDeliveryService> _logger;

    public ScheduledDeliveryService(
        IServiceProvider serviceProvider,
        IOptions<CommunicationOptions> options,
        ILogger<ScheduledDeliveryService> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value.Queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.ScheduledDeliveryEnabled)
        {
            _logger.LogInformation("ScheduledDeliveryService is disabled");
            return;
        }

        _logger.LogInformation("ScheduledDeliveryService started (interval: {Interval}ms)",
            _options.ScheduledPollingIntervalMs);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessScheduledNotifications(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled notifications");
            }

            await Task.Delay(_options.ScheduledPollingIntervalMs, stoppingToken);
        }
    }

    private async Task ProcessScheduledNotifications(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();

        var dueNotifications = await notificationRepository.GetScheduledDueAsync(cancellationToken);

        foreach (var notification in dueNotifications)
        {
            await queueService.EnqueueAsync(notification.Id, cancellationToken);
            _logger.LogInformation("Scheduled notification {NotificationId} moved to queue", notification.Id);
        }
    }
}
