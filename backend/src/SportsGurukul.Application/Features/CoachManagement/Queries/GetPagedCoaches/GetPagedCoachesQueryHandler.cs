using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetPagedCoaches;

public class GetPagedCoachesQueryHandler : IRequestHandler<GetPagedCoachesQuery, Result<CoachSearchResponse>>
{
    private readonly ICoachSearchRepository _searchRepository;
    private readonly ILogger<GetPagedCoachesQueryHandler> _logger;

    public GetPagedCoachesQueryHandler(
        ICoachSearchRepository searchRepository,
        ILogger<GetPagedCoachesQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<CoachSearchResponse>> Handle(GetPagedCoachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paged coaches: Page {Page}, Size {PageSize}", request.Page, request.PageSize);

        var (coaches, totalCount) = await _searchRepository.SearchCoachesAsync(
            null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null,
            request.SortBy,
            request.SortDescending,
            request.Page,
            request.PageSize,
            null,
            false,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var items = coaches.Select(CoachMappingHelper.MapToSummaryDto).ToList();

        var response = new CoachSearchResponse
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        _logger.LogInformation("Retrieved {Count} coaches (Page {Page} of {TotalPages})", items.Count, request.Page, totalPages);

        return Result<CoachSearchResponse>.Success(response);
    }

}
