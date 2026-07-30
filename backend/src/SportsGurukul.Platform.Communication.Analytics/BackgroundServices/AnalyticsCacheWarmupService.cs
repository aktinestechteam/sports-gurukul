using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.BackgroundServices;

public class CacheWarmupOptions
{
    public int RefreshIntervalMinutes { get; set; } = 15;
}

public sealed class AnalyticsCacheWarmupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AnalyticsCacheWarmupService> _logger;
    private readonly CacheWarmupOptions _options;

    public AnalyticsCacheWarmupService(
        IServiceScopeFactory scopeFactory,
        ILogger<AnalyticsCacheWarmupService> logger,
        IOptions<CacheWarmupOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AnalyticsCacheWarmupService started. Refreshing every {Interval} minutes.", _options.RefreshIntervalMinutes);

        await WarmupCacheAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(_options.RefreshIntervalMinutes), stoppingToken);
                await WarmupCacheAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during analytics cache warmup cycle.");
            }
        }
    }

    private async Task WarmupCacheAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
        var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();
        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        try
        {
            _logger.LogInformation("Starting analytics cache warmup...");

            await WarmupAsync("NotificationDashboard", () => dashboardService.GetNotificationDashboardAsync(null, ct), ct);
            await WarmupAsync("CampaignDashboard", () => dashboardService.GetCampaignDashboardAsync(null, ct), ct);
            await WarmupAsync("ProviderDashboard", () => dashboardService.GetProviderDashboardAsync(null, ct), ct);
            await WarmupAsync("QueueDashboard", () => dashboardService.GetQueueDashboardAsync(ct), ct);
            await WarmupAsync("TemplateDashboard", () => dashboardService.GetTemplateDashboardAsync(null, ct), ct);

            await WarmupAsync("AnalyticsSummary", () => analyticsService.GetSummaryAsync(null, ct), ct);

            _logger.LogInformation("Analytics cache warmup completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analytics cache warmup failed.");
        }
    }

    private async Task WarmupAsync(string name, Func<Task> factory, CancellationToken ct)
    {
        try
        {
            _logger.LogDebug("Warming up {CacheSection}...", name);
            await factory();
            _logger.LogDebug("{CacheSection} warmed up successfully.", name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to warm up {CacheSection}.", name);
        }
    }
}
