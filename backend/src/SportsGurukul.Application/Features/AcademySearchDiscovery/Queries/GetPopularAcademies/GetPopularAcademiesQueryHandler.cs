using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularAcademies;

public class GetPopularAcademiesQueryHandler : IRequestHandler<GetPopularAcademiesQuery, Result<IReadOnlyList<PopularAcademyDto>>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<GetPopularAcademiesQueryHandler> _logger;

    public GetPopularAcademiesQueryHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<GetPopularAcademiesQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<PopularAcademyDto>>> Handle(GetPopularAcademiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching popular academies, Limit={Limit}", request.Limit);

        var academies = await _academySearchRepository.GetPopularAcademiesAsync(request.Limit, cancellationToken);

        var results = academies.Select(a => new PopularAcademyDto
        {
            Id = a.Id,
            Name = a.Name,
            AcademyCode = a.AcademyCode,
            LogoUrl = a.LogoUrl,
            City = a.Contact?.City,
            State = a.Contact?.State,
            IsVerified = a.VerificationStatus == Domain.Enums.VerificationStatus.Verified,
            ViewCount = 0,
            AverageRating = 0
        }).ToList();

        return Result<IReadOnlyList<PopularAcademyDto>>.Success(results);
    }
}
