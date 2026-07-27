using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateRankings;

public class GenerateRankingsCommandHandler : IRequestHandler<GenerateRankingsCommand, Result<IReadOnlyList<RankingDto>>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IRankingCalculationService _rankingCalculationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateRankingsCommandHandler> _logger;

    public GenerateRankingsCommandHandler(
        ITournamentRepository tournamentRepository,
        IMatchRepository matchRepository,
        IRankingCalculationService rankingCalculationService,
        IUnitOfWork unitOfWork,
        ILogger<GenerateRankingsCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _matchRepository = matchRepository;
        _rankingCalculationService = rankingCalculationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RankingDto>>> Handle(GenerateRankingsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating rankings for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<IReadOnlyList<RankingDto>>.Failure("Tournament not found.");

        var details = await _tournamentRepository.GetWithDetailsAsync(request.TournamentId, cancellationToken);
        var participants = details?.Participants?.Where(p => p.IsActive).ToList() ?? [];
        var completedMatches = await _matchRepository.GetByStatusAsync(request.TournamentId, MatchStatus.Completed, cancellationToken);

        var rankings = await _rankingCalculationService.CalculateRankingsAsync(tournament, completedMatches, participants, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rankings generated for tournament: {TournamentId}, Count: {Count}", tournament.Id, rankings.Count);

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
