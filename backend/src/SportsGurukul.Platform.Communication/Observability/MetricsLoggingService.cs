using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Observability;

public class MetricsLoggingService : BackgroundService
{
    private readonly DeliveryMetricsCollector _metrics;
    private readonly ObservabilityOptions _options;
    private readonly ILogger<MetricsLoggingService> _logger;

    public MetricsLoggingService(
        DeliveryMetricsCollector metrics,
        IOptions<CommunicationOptions> options,
        ILogger<MetricsLoggingService> logger)
    {
        _metrics = metrics;
        _options = options.Value.Observability;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.MetricsEnabled)
        {
            _logger.LogInformation("MetricsLoggingService is disabled");
            return;
        }

        _logger.LogInformation("MetricsLoggingService started (interval: 60s)");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _metrics.LogMetricsSummary();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging metrics summary");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
