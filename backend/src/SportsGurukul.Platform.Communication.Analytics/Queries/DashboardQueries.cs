using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Queries;

public record GetNotificationDashboardQuery(AnalyticsFilterDto? Filter) : IRequest<NotificationDashboardDto>;

public class GetNotificationDashboardQueryHandler(IDashboardService service) : IRequestHandler<GetNotificationDashboardQuery, NotificationDashboardDto>
{
    public Task<NotificationDashboardDto> Handle(GetNotificationDashboardQuery query, CancellationToken ct)
        => service.GetNotificationDashboardAsync(query.Filter, ct);
}

public record GetCampaignDashboardQuery(AnalyticsFilterDto? Filter) : IRequest<CampaignDashboardDto>;

public class GetCampaignDashboardQueryHandler(IDashboardService service) : IRequestHandler<GetCampaignDashboardQuery, CampaignDashboardDto>
{
    public Task<CampaignDashboardDto> Handle(GetCampaignDashboardQuery query, CancellationToken ct)
        => service.GetCampaignDashboardAsync(query.Filter, ct);
}

public record GetProviderDashboardQuery(AnalyticsFilterDto? Filter) : IRequest<ProviderDashboardDto>;

public class GetProviderDashboardQueryHandler(IDashboardService service) : IRequestHandler<GetProviderDashboardQuery, ProviderDashboardDto>
{
    public Task<ProviderDashboardDto> Handle(GetProviderDashboardQuery query, CancellationToken ct)
        => service.GetProviderDashboardAsync(query.Filter, ct);
}

public record GetQueueDashboardQuery : IRequest<QueueDashboardDto>;

public class GetQueueDashboardQueryHandler(IDashboardService service) : IRequestHandler<GetQueueDashboardQuery, QueueDashboardDto>
{
    public Task<QueueDashboardDto> Handle(GetQueueDashboardQuery query, CancellationToken ct)
        => service.GetQueueDashboardAsync(ct);
}

public record GetTemplateDashboardQuery(AnalyticsFilterDto? Filter) : IRequest<TemplateDashboardDto>;

public class GetTemplateDashboardQueryHandler(IDashboardService service) : IRequestHandler<GetTemplateDashboardQuery, TemplateDashboardDto>
{
    public Task<TemplateDashboardDto> Handle(GetTemplateDashboardQuery query, CancellationToken ct)
        => service.GetTemplateDashboardAsync(query.Filter, ct);
}

public record GetFullDashboardQuery(AnalyticsFilterDto? Filter) : IRequest<DashboardSummaryDto>;

public class GetFullDashboardQueryHandler(IDashboardService service) : IRequestHandler<GetFullDashboardQuery, DashboardSummaryDto>
{
    public Task<DashboardSummaryDto> Handle(GetFullDashboardQuery query, CancellationToken ct)
        => service.GetFullDashboardAsync(query.Filter, ct);
}
