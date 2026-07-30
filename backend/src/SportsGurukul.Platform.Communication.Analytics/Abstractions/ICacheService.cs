using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken ct = default);
    Task<bool> RemoveAsync(string key, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration, CancellationToken ct = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);
    Task ClearAsync(CancellationToken ct = default);
    Task<long> IncrementAsync(string key, long value = 1, CancellationToken ct = default);
    Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken ct = default);
    Task SetManyAsync<T>(Dictionary<string, T> items, TimeSpan? expiration, CancellationToken ct = default);
    double HitRate { get; }
    long Hits { get; }
    long Misses { get; }
}

public static class CacheKeys
{
    public const string TemplatePrefix = "templates:";
    public const string LocalizationPrefix = "localizations:";
    public const string AnalyticsSummaryPrefix = "analytics:summary:";
    public const string DashboardPrefix = "dashboard:";
    public const string SegmentPrefix = "segments:";
    public const string ProviderPrefix = "providers:";
    public const string CampaignPrefix = "campaigns:";
    public const string SchedulePrefix = "schedules:";

    public static string TemplateKey(Guid id) => $"{TemplatePrefix}{id}";
    public static string TemplateRenderKey(Guid id, string? locale) => $"{TemplatePrefix}render:{id}:{locale ?? "default"}";
    public static string LocalizationResourceKey(string locale) => $"{LocalizationPrefix}{locale}";
    public static string AnalyticsSummaryKey(AnalyticsFilterDto filter) => $"{AnalyticsSummaryPrefix}{filter.StartDate:yyyyMMdd}-{filter.EndDate:yyyyMMdd}";
    public static string DashboardKey(string type, AnalyticsFilterDto? filter = null) => $"{DashboardPrefix}{type}:{filter?.StartDate:yyyyMMdd}-{filter?.EndDate:yyyyMMdd}";
    public static string SegmentResultKey(Guid segmentId) => $"{SegmentPrefix}result:{segmentId}";
    public static string ProviderPerformanceKey(Guid providerId) => $"{ProviderPrefix}performance:{providerId}";
}
