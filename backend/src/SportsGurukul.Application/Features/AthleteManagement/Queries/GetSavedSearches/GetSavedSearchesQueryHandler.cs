using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetSavedSearches;

public class GetSavedSearchesQueryHandler : IRequestHandler<GetSavedSearchesQuery, Result<IReadOnlyList<SavedSearchDto>>>
{
    private readonly ISavedSearchRepository _repository;
    private readonly ILogger<GetSavedSearchesQueryHandler> _logger;

    public GetSavedSearchesQueryHandler(
        ISavedSearchRepository repository,
        ILogger<GetSavedSearchesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SavedSearchDto>>> Handle(
        GetSavedSearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching saved searches for user: {UserId}", request.UserId);

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
