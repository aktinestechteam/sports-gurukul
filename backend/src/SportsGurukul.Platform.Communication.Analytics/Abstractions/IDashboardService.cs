using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface IDashboardService
{
    Task<NotificationDashboardDto> GetNotificationDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<CampaignDashboardDto> GetCampaignDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<ProviderDashboardDto> GetProviderDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<QueueDashboardDto> GetQueueDashboardAsync(CancellationToken ct = default);
    Task<TemplateDashboardDto> GetTemplateDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<DashboardSummaryDto> GetFullDashboardAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<DashboardKpiDto>> GetNotificationKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<DashboardKpiDto>> GetCampaignKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<DashboardKpiDto>> GetProviderKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
    Task<List<DashboardKpiDto>> GetQueueKpisAsync(CancellationToken ct = default);
    Task<List<DashboardKpiDto>> GetTemplateKpisAsync(AnalyticsFilterDto? filter, CancellationToken ct = default);
}
