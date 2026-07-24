using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetRecentCoachSearches;

public class GetRecentCoachSearchesQueryHandler : IRequestHandler<GetRecentCoachSearchesQuery, Result<IReadOnlyList<RecentSearchDto>>>
{
    private readonly IRecentSearchRepository _repository;
    private readonly ILogger<GetRecentCoachSearchesQueryHandler> _logger;

    public GetRecentCoachSearchesQueryHandler(
        IRecentSearchRepository repository,
        ILogger<GetRecentCoachSearchesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RecentSearchDto>>> Handle(
        GetRecentCoachSearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching recent coach searches for user: {UserId}", request.UserId);

        var searches = await _repository.GetByUserIdAsync(request.UserId, request.Limit, cancellationToken);
        var dtos = searches.Select(s => new RecentSearchDto
        {
            Id = s.Id,
            QueryText = s.QueryText,
            FiltersJson = s.FiltersJson,
            ResultCount = s.ResultCount,
            SearchedAt = s.SearchedAt
        }).ToList();

        return Result<IReadOnlyList<RecentSearchDto>>.Success(dtos);
    }
}
