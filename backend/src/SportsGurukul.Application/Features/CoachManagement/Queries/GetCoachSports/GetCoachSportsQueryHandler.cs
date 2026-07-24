using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSports;

public class GetCoachSportsQueryHandler : IRequestHandler<GetCoachSportsQuery, Result<IReadOnlyList<SportDto>>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ILogger<GetCoachSportsQueryHandler> _logger;

    public GetCoachSportsQueryHandler(
        ICoachRepository coachRepository,
        ILogger<GetCoachSportsQueryHandler> logger)
    {
        _coachRepository = coachRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SportDto>>> Handle(GetCoachSportsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting sports for coach Id: {CoachId}", request.CoachId);

        var coachSports = await _coachRepository.GetCoachSportsAsync(request.CoachId, cancellationToken);

        var sports = coachSports.Select(cs => new SportDto
        {
            Id = cs.Sport.Id,
            Name = cs.Sport.Name,
            Code = cs.Sport.Code,
            OlympicSport = cs.Sport.OlympicSport,
            CategoryName = cs.Sport.SportCategory?.Name,
            IsPrimarySport = cs.IsPrimarySport,
            JoinedDate = cs.JoinedDate
        }).ToList();

        return Result<IReadOnlyList<SportDto>>.Success(sports);
    }
}
