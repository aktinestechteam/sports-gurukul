using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSports;

public class GetAthleteSportsQueryHandler : IRequestHandler<GetAthleteSportsQuery, Result<IReadOnlyList<SportDto>>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetAthleteSportsQueryHandler> _logger;

    public GetAthleteSportsQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<GetAthleteSportsQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SportDto>>> Handle(GetAthleteSportsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching sports for athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<IReadOnlyList<SportDto>>.Failure("Athlete not found.");
        }

        var athleteSports = await _athleteRepository.GetAthleteSportsAsync(request.AthleteId, cancellationToken);

        var sports = athleteSports.Select(s => new SportDto
        {
            Id = s.Id,
            SportId = s.SportId,
            Name = s.Sport.Name,
            Code = s.Sport.Code,
            CategoryName = s.Sport.SportCategory.Name,
            OlympicSport = s.Sport.OlympicSport,
            IsPrimarySport = s.IsPrimarySport,
            JoinedDate = s.JoinedDate
        }).ToList();

        return Result<IReadOnlyList<SportDto>>.Success(sports);
    }
}
