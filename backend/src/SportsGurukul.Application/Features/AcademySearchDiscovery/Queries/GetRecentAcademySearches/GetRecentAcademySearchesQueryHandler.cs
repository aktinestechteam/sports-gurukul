using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetRecentAcademySearches;

public class GetRecentAcademySearchesQueryHandler : IRequestHandler<GetRecentAcademySearchesQuery, Result<IReadOnlyList<RecentAcademySearchDto>>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<GetRecentAcademySearchesQueryHandler> _logger;

    public GetRecentAcademySearchesQueryHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<GetRecentAcademySearchesQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RecentAcademySearchDto>>> Handle(GetRecentAcademySearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching recent academy searches for user: {UserId}", request.UserId);

        var searches = await _academySearchRepository.GetRecentSearchesAsync(request.UserId, request.Limit, cancellationToken);

        var dtos = searches.Select(s => new RecentAcademySearchDto
        {
            Id = s.Id,
            SearchTerm = s.SearchTerm,
            City = s.City,
            State = s.State,
            SportName = s.SportName,
            AcademyCount = s.AcademyCount,
            SearchedAt = s.SearchedAt
        }).ToList();

        return Result<IReadOnlyList<RecentAcademySearchDto>>.Success(dtos);
    }
}
