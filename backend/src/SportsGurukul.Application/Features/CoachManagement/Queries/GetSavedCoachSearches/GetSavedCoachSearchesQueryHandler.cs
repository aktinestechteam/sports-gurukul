using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetSavedCoachSearches;

public class GetSavedCoachSearchesQueryHandler : IRequestHandler<GetSavedCoachSearchesQuery, Result<IReadOnlyList<SavedSearchDto>>>
{
    private readonly ISavedSearchRepository _repository;
    private readonly ILogger<GetSavedCoachSearchesQueryHandler> _logger;

    public GetSavedCoachSearchesQueryHandler(
        ISavedSearchRepository repository,
        ILogger<GetSavedCoachSearchesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SavedSearchDto>>> Handle(
        GetSavedCoachSearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching saved coach searches for user: {UserId}", request.UserId);

        var searches = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        var dtos = searches.Select(s => new SavedSearchDto
        {
            Id = s.Id,
            Name = s.Name,
            FiltersJson = s.FiltersJson,
            UsageCount = s.UsageCount,
            CreatedAt = s.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<SavedSearchDto>>.Success(dtos);
    }
}
