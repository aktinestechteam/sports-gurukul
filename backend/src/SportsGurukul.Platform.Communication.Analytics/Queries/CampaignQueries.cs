using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Queries;

public record GetCampaignQuery(Guid Id) : IRequest<CampaignDetailDto>;

public class GetCampaignQueryHandler(ICampaignManagementService service) : IRequestHandler<GetCampaignQuery, CampaignDetailDto>
{
    public Task<CampaignDetailDto> Handle(GetCampaignQuery query, CancellationToken ct)
        => service.GetByIdAsync(query.Id, ct);
}

public record SearchCampaignsQuery(CampaignSearchCriteria Criteria) : IRequest<CampaignSearchResult>;

public class SearchCampaignsQueryHandler(ICampaignManagementService service) : IRequestHandler<SearchCampaignsQuery, CampaignSearchResult>
{
    public Task<CampaignSearchResult> Handle(SearchCampaignsQuery query, CancellationToken ct)
        => service.SearchAsync(query.Criteria, ct);
}

public record GetDueCampaignsQuery : IRequest<List<CampaignDetailDto>>;

public class GetDueCampaignsQueryHandler(ICampaignManagementService service) : IRequestHandler<GetDueCampaignsQuery, List<CampaignDetailDto>>
{
    public Task<List<CampaignDetailDto>> Handle(GetDueCampaignsQuery query, CancellationToken ct)
        => service.GetDueCampaignsAsync(ct);
}

public record GetCampaignCountByStatusQuery(CampaignStatus Status) : IRequest<int>;

public class GetCampaignCountByStatusQueryHandler(ICampaignManagementService service) : IRequestHandler<GetCampaignCountByStatusQuery, int>
{
    public Task<int> Handle(GetCampaignCountByStatusQuery query, CancellationToken ct)
        => service.GetCountByStatusAsync(query.Status, ct);
}
