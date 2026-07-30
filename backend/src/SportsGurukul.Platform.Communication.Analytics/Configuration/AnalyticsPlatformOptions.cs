namespace SportsGurukul.Platform.Communication.Analytics.Configuration;

public class AnalyticsPlatformOptions
{
    public bool EnableTemplateCaching { get; set; } = true;
    public bool EnableLocalizationCaching { get; set; } = true;
    public bool EnableAnalyticsCaching { get; set; } = true;
    public bool EnableDashboardCaching { get; set; } = true;
    public bool EnableSegmentCaching { get; set; } = true;

    public TimeSpan TemplateCacheDuration { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan LocalizationCacheDuration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan AnalyticsCacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan DashboardCacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan SegmentCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 100;

    public bool EnableBackgroundProcessing { get; set; } = true;
    public bool EnableCacheWarmup { get; set; } = true;
    public int ScheduleCheckIntervalSeconds { get; set; } = 30;
    public int CacheWarmupIntervalMinutes { get; set; } = 15;

    public string BiToolExtensionPoint { get; set; } = "PowerBI,Tableau,Looker";
    public bool ExportEnabled { get; set; } = true;
}
