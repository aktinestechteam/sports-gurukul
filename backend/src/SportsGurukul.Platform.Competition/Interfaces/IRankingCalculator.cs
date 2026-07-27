using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IRankingCalculator
{
    Task<IReadOnlyList<Ranking>> CalculateRankingsAsync(
        CompetitionConfig config,
        IReadOnlyList<CompetitionMatch> completedMatches,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ranking>> CalculateRoundRobinRankingsAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ranking>> CalculateLeagueRankingsAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default);
}
