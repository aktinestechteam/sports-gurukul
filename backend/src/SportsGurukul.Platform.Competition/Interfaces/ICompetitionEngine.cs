using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ICompetitionEngine
{
    Task<CompetitionResult> GenerateCompetitionAsync(
        CompetitionConfig config,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompetitionMatch>> AdvanceWinnerAsync(
        CompetitionMatch completedMatch,
        IReadOnlyList<CompetitionMatch> allMatches,
        CancellationToken cancellationToken = default);

    Task<RankingResult> CalculateRankingsAsync(
        CompetitionConfig config,
        IReadOnlyList<CompetitionMatch> completedMatches,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default);
}

public class CompetitionResult
{
    public List<Bracket> Brackets { get; set; } = new();
    public List<Fixture> Fixtures { get; set; } = new();
    public List<Seed> Seeds { get; set; } = new();
    public List<CompetitionMatch> Matches { get; set; } = new();
    public CompetitionFormat Format { get; set; }
}

public class RankingResult
{
    public List<Ranking> Rankings { get; set; } = new();
    public List<Ranking> MedalStandings { get; set; } = new();
}
