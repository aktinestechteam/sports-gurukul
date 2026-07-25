using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.SearchCoaches;

public class SearchCoachesQueryHandler : IRequestHandler<SearchCoachesQuery, Result<CoachSearchResponse>>
{
    private readonly ICoachSearchRepository _searchRepository;
    private readonly ILogger<SearchCoachesQueryHandler> _logger;

    public SearchCoachesQueryHandler(
        ICoachSearchRepository searchRepository,
        ILogger<SearchCoachesQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<CoachSearchResponse>> Handle(SearchCoachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching coaches with filters");

        var (coaches, totalCount) = await _searchRepository.SearchCoachesAsync(
            request.SearchTerm,
            request.Name,
            null,
            null,
            null,
            request.SportName,
            null,
            null,
            request.CoachingLevel,
            request.MinExperience,
            request.MaxExperience,
            request.CertificationName,
            null,
            null,
            null,
            request.Country,
            request.State,
            request.City,
            null,
            null,
            null,
            null,
            null,
            request.OnlineAvailable,
            request.OfflineAvailable,
            request.VerificationStatus.HasValue
                ? request.VerificationStatus == VerificationStatus.Verified
                : null,
            null,
            request.Language,
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

        _logger.LogInformation("Search returned {Count} coaches (Page {Page} of {TotalPages})", items.Count, request.Page, totalPages);

        return Result<CoachSearchResponse>.Success(response);
    }

}
