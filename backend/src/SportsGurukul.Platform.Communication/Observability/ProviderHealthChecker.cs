using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Observability;

public class ProviderHealthChecker : BackgroundService
{
    private readonly INotificationProviderFactory _providerFactory;
    private readonly ObservabilityOptions _options;
    private readonly ILogger<ProviderHealthChecker> _logger;

    public ProviderHealthChecker(
        INotificationProviderFactory providerFactory,
        IOptions<CommunicationOptions> options,
        ILogger<ProviderHealthChecker> logger)
    {
        _providerFactory = providerFactory;
        _options = options.Value.Observability;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.HealthChecksEnabled)
        {
            _logger.LogInformation("ProviderHealthChecker is disabled");
            return;
        }

        _logger.LogInformation("ProviderHealthChecker started (interval: {Interval}s)",
            _options.HealthCheckIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAllProviders(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during provider health check cycle");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.HealthCheckIntervalSeconds), stoppingToken);
        }
    }

    private async Task CheckAllProviders(CancellationToken cancellationToken)
    {
        var providers = _providerFactory.GetAllProviders();

        foreach (var provider in providers)
        {
            try
            {
                var isHealthy = await provider.HealthCheckAsync(cancellationToken);
                _logger.LogInformation("Provider {Provider} health: {Status}",
                    provider.Name, isHealthy ? "Healthy" : "Unhealthy");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Provider {Provider} health check threw exception", provider.Name);
            }
        }
    }
}
