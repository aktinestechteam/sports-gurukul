using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.BackgroundServices;
using SportsGurukul.Platform.Communication.Analytics.Configuration;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Platform.Communication.Analytics;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new AnalyticsPlatformOptions();
        configuration.GetSection("AnalyticsPlatform").Bind(options);
        services.AddSingleton(options);

        services.AddSingleton<ICacheService, CacheService>();

        services.AddSingleton<ITemplateManagementService, TemplateManagementService>();
        services.AddSingleton<ITemplateVersionService, TemplateVersionService>();
        services.AddSingleton<ICampaignManagementService, CampaignManagementService>();
        services.AddSingleton<ISchedulingEngine, SchedulingEngine>();
        services.AddSingleton<IAudienceSegmentationService, AudienceSegmentationService>();
        services.AddSingleton<IAnalyticsService, AnalyticsService>();
        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<ISearchService, SearchService>();

        if (options.EnableBackgroundProcessing)
        {
            services.AddHostedService<ScheduleExecutionService>();
        }

        if (options.EnableCacheWarmup)
        {
            services.AddHostedService<AnalyticsCacheWarmupService>();
        }

        return services;
    }
}
