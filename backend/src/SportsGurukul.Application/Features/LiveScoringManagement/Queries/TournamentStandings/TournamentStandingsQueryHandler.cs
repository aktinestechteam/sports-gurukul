using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.TournamentStandings;

public class TournamentStandingsQueryHandler : IRequestHandler<TournamentStandingsQuery, Result<StandingsDto>>
{
    private readonly IStandingsService _standingsService;
    private readonly ILogger<TournamentStandingsQueryHandler> _logger;

    public TournamentStandingsQueryHandler(IStandingsService standingsService, ILogger<TournamentStandingsQueryHandler> logger)
    {
        _standingsService = standingsService;
        _logger = logger;
    }

    public async Task<Result<StandingsDto>> Handle(TournamentStandingsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting standings for tournament {TournamentId}", request.TournamentId);

        var entries = await _standingsService.GetTournamentStandingsAsync(request.TournamentId, request.SportCode, cancellationToken);

        var dto = new StandingsDto
        {
            TournamentId = request.TournamentId,
            Entries = entries.Select(e => new StandingsEntryDto
            {
                Position = e.Position,
                ParticipantId = e.ParticipantId,
                ParticipantName = e.ParticipantName,
                AcademyName = e.AcademyName,
                Points = e.Points,
                Played = e.Played,
                Won = e.Won,
                Lost = e.Lost,
                Drawn = e.Drawn,
                GoalsFor = e.GoalsFor,
                GoalsAgainst = e.GoalsAgainst,
                GoalDifference = e.GoalDifference
            }).ToList()
        };

        return Result<StandingsDto>.Success(dto);
    }
}
