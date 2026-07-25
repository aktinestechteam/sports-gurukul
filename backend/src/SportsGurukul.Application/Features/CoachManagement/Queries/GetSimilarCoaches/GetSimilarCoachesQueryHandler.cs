using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetSimilarCoaches;

public class GetSimilarCoachesQueryHandler : IRequestHandler<GetSimilarCoachesQuery, Result<IReadOnlyList<SimilarCoachDto>>>
{
    private readonly ICoachSearchRepository _searchRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetSimilarCoachesQueryHandler> _logger;

    private const string CachePrefix = "coach_similar_";

    public GetSimilarCoachesQueryHandler(
        ICoachSearchRepository searchRepository,
        ICacheService cacheService,
        ILogger<GetSimilarCoachesQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SimilarCoachDto>>> Handle(
        GetSimilarCoachesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CachePrefix}{request.CoachId}_{request.Limit}";

        var cached = await _cacheService.GetAsync<List<SimilarCoachDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Result<IReadOnlyList<SimilarCoachDto>>.Success(cached);

        _logger.LogInformation("Fetching similar coaches for coach: {CoachId}", request.CoachId);

        var coaches = await _searchRepository.GetSimilarCoachesAsync(
            request.CoachId, request.Limit, cancellationToken);

        var referenceSportIds = coaches.SelectMany(c => c.CoachSports?.Select(cs => cs.SportId) ?? Enumerable.Empty<Guid>()).ToHashSet();

        var dtos = coaches.Select(c => new SimilarCoachDto
        {
            Id = c.Id,
            FullName = c.User?.FullName ?? string.Empty,
            CoachCode = c.CoachCode,
            ProfileImageUrl = c.User?.ProfileImageUrl,
            CoachingLevel = c.CoachingLevel.ToString(),
            YearsOfExperience = c.YearsOfExperience,
            PrimarySport = c.CoachSports?.FirstOrDefault(cs => cs.IsPrimarySport)?.Sport?.Name,
            City = c.Location?.City,
            State = c.Location?.State,
            IsVerified = c.VerificationStatus == VerificationStatus.Verified,
            MatchScore = c.CoachSports?.Count(cs => referenceSportIds.Contains(cs.SportId)) ?? 0
        }).ToList();

        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);

        return Result<IReadOnlyList<SimilarCoachDto>>.Success(dtos);
    }
}
