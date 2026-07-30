using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Queries;

public record GetTemplateQuery(Guid Id) : IRequest<TemplateDetailDto>;

public class GetTemplateQueryHandler(ITemplateManagementService service) : IRequestHandler<GetTemplateQuery, TemplateDetailDto>
{
    public Task<TemplateDetailDto> Handle(GetTemplateQuery query, CancellationToken ct)
        => service.GetByIdAsync(query.Id, ct);
}

public record SearchTemplatesQuery(TemplateSearchCriteria Criteria) : IRequest<TemplateSearchResult>;

public class SearchTemplatesQueryHandler(ITemplateManagementService service) : IRequestHandler<SearchTemplatesQuery, TemplateSearchResult>
{
    public Task<TemplateSearchResult> Handle(SearchTemplatesQuery query, CancellationToken ct)
        => service.SearchAsync(query.Criteria, ct);
}

public record GetTemplateVersionsQuery(Guid TemplateId) : IRequest<List<TemplateVersionDetailDto>>;

public class GetTemplateVersionsQueryHandler(ITemplateManagementService service) : IRequestHandler<GetTemplateVersionsQuery, List<TemplateVersionDetailDto>>
{
    public Task<List<TemplateVersionDetailDto>> Handle(GetTemplateVersionsQuery query, CancellationToken ct)
        => service.GetVersionsAsync(query.TemplateId, ct);
}

public record CompareTemplateVersionsQuery(Guid TemplateId, int FromVersion, int ToVersion) : IRequest<TemplateVersionCompareDto>;

public class CompareTemplateVersionsQueryHandler(ITemplateManagementService service) : IRequestHandler<CompareTemplateVersionsQuery, TemplateVersionCompareDto>
{
    public Task<TemplateVersionCompareDto> Handle(CompareTemplateVersionsQuery query, CancellationToken ct)
        => service.CompareVersionsAsync(query.TemplateId, query.FromVersion, query.ToVersion, ct);
}

public record GetTemplateLocalizationsQuery(Guid TemplateId) : IRequest<List<TemplateLocalizationDto>>;

public class GetTemplateLocalizationsQueryHandler(ITemplateManagementService service) : IRequestHandler<GetTemplateLocalizationsQuery, List<TemplateLocalizationDto>>
{
    public Task<List<TemplateLocalizationDto>> Handle(GetTemplateLocalizationsQuery query, CancellationToken ct)
        => service.GetLocalizationsAsync(query.TemplateId, ct);
}
