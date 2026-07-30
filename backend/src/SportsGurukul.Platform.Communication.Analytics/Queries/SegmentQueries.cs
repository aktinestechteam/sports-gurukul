using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Queries;

public record GetSegmentQuery(Guid Id) : IRequest<SegmentDefinitionDto>;

public class GetSegmentQueryHandler(IAudienceSegmentationService service) : IRequestHandler<GetSegmentQuery, SegmentDefinitionDto>
{
    public Task<SegmentDefinitionDto> Handle(GetSegmentQuery query, CancellationToken ct)
        => service.GetByIdAsync(query.Id, ct);
}

public record SearchSegmentsQuery(SegmentSearchCriteria Criteria) : IRequest<SegmentSearchResult>;

public class SearchSegmentsQueryHandler(IAudienceSegmentationService service) : IRequestHandler<SearchSegmentsQuery, SegmentSearchResult>
{
    public Task<SegmentSearchResult> Handle(SearchSegmentsQuery query, CancellationToken ct)
        => service.SearchAsync(query.Criteria, ct);
}

public record EvaluateSegmentQuery(Guid? SegmentId, SegmentDefinitionDto? Definition) : IRequest<SegmentResultDto>;

public class EvaluateSegmentQueryHandler(IAudienceSegmentationService service) : IRequestHandler<EvaluateSegmentQuery, SegmentResultDto>
{
    public Task<SegmentResultDto> Handle(EvaluateSegmentQuery query, CancellationToken ct)
    {
        if (query.SegmentId.HasValue)
            return service.EvaluateSegmentAsync(query.SegmentId.Value, ct);
        if (query.Definition is not null)
            return service.EvaluateSegmentDefinitionAsync(query.Definition, ct);
        throw new ArgumentException("Either SegmentId or Definition must be provided.");
    }
}

public record PreviewSegmentQuery(SegmentPreviewRequest Request) : IRequest<SegmentPreviewResult>;

public class PreviewSegmentQueryHandler(IAudienceSegmentationService service) : IRequestHandler<PreviewSegmentQuery, SegmentPreviewResult>
{
    public Task<SegmentPreviewResult> Handle(PreviewSegmentQuery query, CancellationToken ct)
        => service.PreviewAsync(query.Request, ct);
}

public record GetSavedSegmentsQuery : IRequest<List<SavedSegmentDto>>;

public class GetSavedSegmentsQueryHandler(IAudienceSegmentationService service) : IRequestHandler<GetSavedSegmentsQuery, List<SavedSegmentDto>>
{
    public Task<List<SavedSegmentDto>> Handle(GetSavedSegmentsQuery query, CancellationToken ct)
        => service.GetSavedSegmentsAsync(ct);
}

public record GetAvailableRulesQuery : IRequest<List<RuleDefinitionDto>>;

public class GetAvailableRulesQueryHandler(IAudienceSegmentationService service) : IRequestHandler<GetAvailableRulesQuery, List<RuleDefinitionDto>>
{
    public Task<List<RuleDefinitionDto>> Handle(GetAvailableRulesQuery query, CancellationToken ct)
        => service.GetAvailableRulesAsync(ct);
}
