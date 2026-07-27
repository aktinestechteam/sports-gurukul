using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentRankings;

public class GetTournamentRankingsQueryHandler : IRequestHandler<GetTournamentRankingsQuery, Result<IReadOnlyList<RankingDto>>>
{
    private readonly IRankingRepository _rankingRepository;
    private readonly ILogger<GetTournamentRankingsQueryHandler> _logger;

    public GetTournamentRankingsQueryHandler(
        IRankingRepository rankingRepository,
        ILogger<GetTournamentRankingsQueryHandler> logger)
    {
        _rankingRepository = rankingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RankingDto>>> Handle(GetTournamentRankingsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting rankings for tournament: {TournamentId}", request.TournamentId);

        IReadOnlyList<Domain.Entities.TournamentRanking> rankings;

        if (request.CategoryId.HasValue)
            rankings = await _rankingRepository.GetByCategoryIdAsync(request.CategoryId.Value, cancellationToken);
        else
            rankings = await _rankingRepository.GetByTournamentIdAsync(request.TournamentId, cancellationToken);

        var dtos = rankings.Select(r => new RankingDto
        {
            Id = r.Id,
            TournamentId = r.TournamentId,
            CategoryId = r.CategoryId,
            ParticipantId = r.ParticipantId,
            Rank = r.Rank,
            Points = r.Points,
            Wins = r.Wins,
            Losses = r.Losses,
            Draws = r.Draws,
            MatchesPlayed = r.MatchesPlayed,
            SetsWon = r.SetsWon,
            SetsLost = r.SetsLost,
            GamesWon = r.GamesWon,
            GamesLost = r.GamesLost
        }).ToList();

        return Result<IReadOnlyList<RankingDto>>.Success(dtos);
    }
}
