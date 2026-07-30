using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Queries;

public record GetAnalyticsSummaryQuery(AnalyticsFilterDto? Filter) : IRequest<AnalyticsSummaryDto>;

public class GetAnalyticsSummaryQueryHandler(IAnalyticsService service) : IRequestHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryDto>
{
    public Task<AnalyticsSummaryDto> Handle(GetAnalyticsSummaryQuery query, CancellationToken ct)
        => service.GetSummaryAsync(query.Filter, ct);
}

public record GetDeliveryRateQuery(AnalyticsFilterDto? Filter) : IRequest<DeliveryRateDto>;

public class GetDeliveryRateQueryHandler(IAnalyticsService service) : IRequestHandler<GetDeliveryRateQuery, DeliveryRateDto>
{
    public Task<DeliveryRateDto> Handle(GetDeliveryRateQuery query, CancellationToken ct)
        => service.GetDeliveryRateAsync(query.Filter, ct);
}

public record GetEngagementRateQuery(AnalyticsFilterDto? Filter) : IRequest<EngagementRateDto>;

public class GetEngagementRateQueryHandler(IAnalyticsService service) : IRequestHandler<GetEngagementRateQuery, EngagementRateDto>
{
    public Task<EngagementRateDto> Handle(GetEngagementRateQuery query, CancellationToken ct)
        => service.GetEngagementRateAsync(query.Filter, ct);
}

public record GetProviderPerformanceQuery(AnalyticsFilterDto? Filter) : IRequest<List<ProviderPerformanceDto>>;

public class GetProviderPerformanceQueryHandler(IAnalyticsService service) : IRequestHandler<GetProviderPerformanceQuery, List<ProviderPerformanceDto>>
{
    public Task<List<ProviderPerformanceDto>> Handle(GetProviderPerformanceQuery query, CancellationToken ct)
        => service.GetProviderPerformanceAsync(query.Filter, ct);
}

public record GetChannelPerformanceQuery(AnalyticsFilterDto? Filter) : IRequest<List<ChannelPerformanceDto>>;

public class GetChannelPerformanceQueryHandler(IAnalyticsService service) : IRequestHandler<GetChannelPerformanceQuery, List<ChannelPerformanceDto>>
{
    public Task<List<ChannelPerformanceDto>> Handle(GetChannelPerformanceQuery query, CancellationToken ct)
        => service.GetChannelPerformanceAsync(query.Filter, ct);
}

public record GetCampaignPerformanceQuery(AnalyticsFilterDto? Filter) : IRequest<List<CampaignPerformanceDto>>;

public class GetCampaignPerformanceQueryHandler(IAnalyticsService service) : IRequestHandler<GetCampaignPerformanceQuery, List<CampaignPerformanceDto>>
{
    public Task<List<CampaignPerformanceDto>> Handle(GetCampaignPerformanceQuery query, CancellationToken ct)
        => service.GetCampaignPerformanceAsync(query.Filter, ct);
}

public record GetTimeSeriesQuery(string Metric, AnalyticsFilterDto Filter) : IRequest<List<TimeSeriesPointDto>>;

public class GetTimeSeriesQueryHandler(IAnalyticsService service) : IRequestHandler<GetTimeSeriesQuery, List<TimeSeriesPointDto>>
{
    public Task<List<TimeSeriesPointDto>> Handle(GetTimeSeriesQuery query, CancellationToken ct)
        => service.GetTimeSeriesAsync(query.Metric, query.Filter, ct);
}

public record GetTrendQuery(string Metric, AnalyticsFilterDto Filter) : IRequest<TrendAnalysisDto>;

public class GetTrendQueryHandler(IAnalyticsService service) : IRequestHandler<GetTrendQuery, TrendAnalysisDto>
{
    public Task<TrendAnalysisDto> Handle(GetTrendQuery query, CancellationToken ct)
        => service.GetTrendAsync(query.Metric, query.Filter, ct);
}

public record GenerateAnalyticsReportQuery(AnalyticsFilterDto? Filter) : IRequest<AnalyticsReportDto>;

public class GenerateAnalyticsReportQueryHandler(IAnalyticsService service) : IRequestHandler<GenerateAnalyticsReportQuery, AnalyticsReportDto>
{
    public Task<AnalyticsReportDto> Handle(GenerateAnalyticsReportQuery query, CancellationToken ct)
        => service.GenerateReportAsync(query.Filter, ct);
}
