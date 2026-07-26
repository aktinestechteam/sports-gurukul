using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSimilarAcademies;

public class GetSimilarAcademiesQueryHandler : IRequestHandler<GetSimilarAcademiesQuery, Result<IReadOnlyList<AcademySimilarDto>>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly IAcademyRepository _academyRepository;
    private readonly ILogger<GetSimilarAcademiesQueryHandler> _logger;

    public GetSimilarAcademiesQueryHandler(
        IAcademySearchRepository academySearchRepository,
        IAcademyRepository academyRepository,
        ILogger<GetSimilarAcademiesQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _academyRepository = academyRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AcademySimilarDto>>> Handle(GetSimilarAcademiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching similar academies for AcademyId: {AcademyId}", request.AcademyId);

        var sourceAcademy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (sourceAcademy is null)
            return Result<IReadOnlyList<AcademySimilarDto>>.Failure("Source academy not found.");

        var sourceSportNames = sourceAcademy.AcademySports?
            .Select(s => s.Sport?.Name ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        var sourceFacilityTypes = sourceAcademy.Facilities?
            .Select(f => f.FacilityType.ToString())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        var candidates = await _academySearchRepository.GetSimilarAcademiesAsync(
            request.AcademyId, request.Limit, cancellationToken);

        var totalFeatures = sourceSportNames.Count + sourceFacilityTypes.Count;
        if (totalFeatures == 0)
            totalFeatures = 1;

        var results = candidates.Select(a =>
        {
            var candidateSportNames = a.AcademySports?
                .Select(s => s.Sport?.Name ?? string.Empty)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList() ?? [];

            var candidateFacilityTypes = a.Facilities?
                .Select(f => f.FacilityType.ToString())
                .ToList() ?? [];

            var commonSports = candidateSportNames
                .Where(s => sourceSportNames.Contains(s))
                .ToList();

            var commonFacilities = candidateFacilityTypes
                .Where(f => sourceFacilityTypes.Contains(f))
                .ToList();

            var sharedCount = commonSports.Count + commonFacilities.Count;
            var similarityScore = (double)sharedCount / totalFeatures;

            return new AcademySimilarDto
            {
                Id = a.Id,
                Name = a.Name,
                AcademyCode = a.AcademyCode,
                LogoUrl = a.LogoUrl,
                Description = a.Description,
                City = a.Contact?.City,
                State = a.Contact?.State,
                IsVerified = a.VerificationStatus == Domain.Enums.VerificationStatus.Verified,
                CommonSports = commonSports,
                CommonFacilities = commonFacilities,
                SimilarityScore = Math.Round(similarityScore, 2)
            };
        }).ToList();

        return Result<IReadOnlyList<AcademySimilarDto>>.Success(results);
    }
}
