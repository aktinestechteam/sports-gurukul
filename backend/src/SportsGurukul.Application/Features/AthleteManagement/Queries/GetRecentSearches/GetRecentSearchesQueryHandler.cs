using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetRecentSearches;

public class GetRecentSearchesQueryHandler : IRequestHandler<GetRecentSearchesQuery, Result<IReadOnlyList<RecentSearchDto>>>
{
    private readonly IRecentSearchRepository _repository;
    private readonly ILogger<GetRecentSearchesQueryHandler> _logger;

    public GetRecentSearchesQueryHandler(
        IRecentSearchRepository repository,
        ILogger<GetRecentSearchesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RecentSearchDto>>> Handle(
        GetRecentSearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching recent searches for user: {UserId}", request.UserId);

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
