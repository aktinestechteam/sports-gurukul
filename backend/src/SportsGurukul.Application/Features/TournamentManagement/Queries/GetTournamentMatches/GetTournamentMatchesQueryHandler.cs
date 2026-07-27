using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentMatches;

public class GetTournamentMatchesQueryHandler : IRequestHandler<GetTournamentMatchesQuery, Result<IReadOnlyList<MatchDto>>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ILogger<GetTournamentMatchesQueryHandler> _logger;

    public GetTournamentMatchesQueryHandler(
        IMatchRepository matchRepository,
        ILogger<GetTournamentMatchesQueryHandler> logger)
    {
        _matchRepository = matchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<MatchDto>>> Handle(GetTournamentMatchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting matches for tournament: {TournamentId}", request.TournamentId);

        IReadOnlyList<Domain.Entities.TournamentMatch> matches;

        if (request.Status.HasValue)
        {
            matches = await _matchRepository.GetByStatusAsync(request.TournamentId, request.Status.Value, cancellationToken);
        }
        else if (request.RoundId.HasValue)
        {
            matches = await _matchRepository.GetByRoundIdAsync(request.RoundId.Value, cancellationToken);
        }
        else
        {
            matches = await _matchRepository.GetByTournamentIdAsync(request.TournamentId, cancellationToken);
        }

        var dtos = matches.Select(m => new MatchDto
        {
            Id = m.Id,
            TournamentId = m.TournamentId,
            TournamentStageId = m.TournamentStageId,
            TournamentRoundId = m.TournamentRoundId,
            MatchNumber = m.MatchNumber,
            HomeParticipantId = m.HomeParticipantId,
            HomeParticipantName = m.HomeParticipantName,
            AwayParticipantId = m.AwayParticipantId,
            AwayParticipantName = m.AwayParticipantName,
            ScheduledDate = m.ScheduledDate,
            ScheduledTime = m.ScheduledTime,
            Status = m.Status,
            HomeScore = m.HomeScore,
            AwayScore = m.AwayScore,
            ScoreDetails = m.ScoreDetails,
            WinnerId = m.WinnerId,
            WinnerName = m.WinnerName,
            Notes = m.Notes,
            CreatedAt = m.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<MatchDto>>.Success(dtos);
    }
}
