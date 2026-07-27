using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.TournamentManagement.Services;

public interface IRankingCalculationService
{
    Task<IReadOnlyList<TournamentRanking>> CalculateRankingsAsync(
        Tournament tournament,
        IReadOnlyList<TournamentMatch> completedMatches,
        IReadOnlyList<TournamentParticipant> participants,
        CancellationToken cancellationToken = default);
}
