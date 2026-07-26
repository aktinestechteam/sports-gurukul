using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetRegisteredAthletes;

public class GetRegisteredAthletesQueryHandler : IRequestHandler<GetRegisteredAthletesQuery, Result<IReadOnlyList<AcademyAthleteSummaryDto>>>
{
    private readonly IAthleteAcademyRepository _athleteAcademyRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetRegisteredAthletesQueryHandler> _logger;

    public GetRegisteredAthletesQueryHandler(
        IAthleteAcademyRepository athleteAcademyRepository,
        IAthleteRepository athleteRepository,
        ILogger<GetRegisteredAthletesQueryHandler> logger)
    {
        _athleteAcademyRepository = athleteAcademyRepository;
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AcademyAthleteSummaryDto>>> Handle(GetRegisteredAthletesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching registered athletes for academy: {AcademyId}", request.AcademyId);

        var athleteAcademies = await _athleteAcademyRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);

        var dtos = new List<AcademyAthleteSummaryDto>();
        foreach (var aa in athleteAcademies)
        {
            var athlete = await _athleteRepository.GetByIdWithDetailsAsync(aa.AthleteId, cancellationToken);
            if (athlete is null) continue;

            dtos.Add(new AcademyAthleteSummaryDto
            {
                Id = aa.Id,
                AthleteId = athlete.Id,
                AthleteCode = athlete.AthleteCode,
                FullName = athlete.User.FullName,
                Email = athlete.User.Email,
                PhoneNumber = athlete.User.PhoneNumber,
                ProfileImageUrl = athlete.User.ProfileImageUrl,
                CurrentLevel = athlete.CurrentLevel.ToString(),
                Status = athlete.Status.ToString(),
                PrimarySport = athlete.AthleteSports?
                    .FirstOrDefault(s => s.IsPrimarySport)?.Sport?.Name,
                RegisteredDate = aa.RegisteredDate
            });
        }

        _logger.LogInformation("Retrieved {Count} registered athletes for academy: {AcademyId}", dtos.Count, request.AcademyId);

        return Result<IReadOnlyList<AcademyAthleteSummaryDto>>.Success(dtos);
    }
}
