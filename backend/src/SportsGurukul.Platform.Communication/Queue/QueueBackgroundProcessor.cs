using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;

namespace SportsGurukul.Platform.Communication.Queue;

public class QueueBackgroundProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly QueueOptions _options;
    private readonly ILogger<QueueBackgroundProcessor> _logger;

    public QueueBackgroundProcessor(
        IServiceProvider serviceProvider,
        IOptions<CommunicationOptions> options,
        ILogger<QueueBackgroundProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value.Queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QueueBackgroundProcessor started (interval: {Interval}ms, batch: {Batch})",
            _options.PollingIntervalMs, _options.BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingItems(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in queue background processing cycle");
            }

            await Task.Delay(_options.PollingIntervalMs, stoppingToken);
        }
    }

    private async Task ProcessPendingItems(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<PriorityQueueProcessor>();
        var deadLetterHandler = scope.ServiceProvider.GetRequiredService<DeadLetterQueueHandler>();

        await deadLetterHandler.ProcessDeadLetterQueueAsync(cancellationToken);
        await processor.ProcessQueueItemsAsync(cancellationToken);
    }
}
