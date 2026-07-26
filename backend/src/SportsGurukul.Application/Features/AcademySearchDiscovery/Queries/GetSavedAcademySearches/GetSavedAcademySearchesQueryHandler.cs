using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSavedAcademySearches;

public class GetSavedAcademySearchesQueryHandler : IRequestHandler<GetSavedAcademySearchesQuery, Result<IReadOnlyList<SavedAcademySearchDto>>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<GetSavedAcademySearchesQueryHandler> _logger;

    public GetSavedAcademySearchesQueryHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<GetSavedAcademySearchesQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SavedAcademySearchDto>>> Handle(GetSavedAcademySearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching saved academy searches for user: {UserId}", request.UserId);

        var searches = await _academySearchRepository.GetSavedSearchesAsync(request.UserId, cancellationToken);

        var dtos = searches.Select(s => new SavedAcademySearchDto
        {
            Id = s.Id,
            SearchName = s.SearchName,
            SearchTerm = s.SearchTerm,
            City = s.City,
            State = s.State,
            SportName = s.SportName,
            FacilityType = s.FacilityType,
            VerifiedOnly = s.VerifiedOnly,
            MinMembershipPrice = s.MinMembershipPrice,
            MaxMembershipPrice = s.MaxMembershipPrice,
            ResultCount = s.ResultCount,
            CreatedAt = s.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<SavedAcademySearchDto>>.Success(dtos);
    }
}
