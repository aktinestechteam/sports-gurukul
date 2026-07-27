using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services.Providers;

public class CricketSportRuleProvider : ISportRuleProvider
{
    public string SportCode => "CRICKET";
    public string SportName => "Cricket";

    public SportScoringConfig GetScoringConfig() => new()
    {
        SportCode = SportCode,
        SportName = SportName,
        PrimaryUnit = ScoringUnit.Point,
        SupportedUnits = new List<ScoringUnit> { ScoringUnit.Point, ScoringUnit.Inning },
        PointsForWin = 2,
        PointsForDraw = 1,
        PointsForLoss = 0,
        HasInnings = true,
        MaxInnings = 2,
        AllowsDraws = true,
        TieBreakers = new List<string> { "Net Run Rate", "Head-to-Head" }
    };

    public int CalculateScore(LiveScoreEvent scoreEvent, MatchScore currentScore)
    {
        return currentScore.TotalPoints + scoreEvent.Points;
    }

    public bool DetermineWinner(LiveMatch match) => match.HomeScore.TotalPoints != match.AwayScore.TotalPoints;

    public Guid? DetermineWinner(Guid homeParticipantId, int homeScore, Guid awayParticipantId, int awayScore)
    {
        if (homeScore > awayScore) return homeParticipantId;
        if (awayScore > homeScore) return awayParticipantId;
        return null;
    }

    public IReadOnlyList<string> GetTieBreakers() => new List<string> { "Net Run Rate", "Head-to-Head" };

    public MatchScore CreateEmptyScore() => new();

    public bool IsMatchComplete(LiveMatch match) => match.CurrentPeriod >= GetScoringConfig().MaxInnings;

    public List<ScoringBreakdown> GetScoreBreakdown(MatchScore score) => new()
    {
        new ScoringBreakdown { Unit = ScoringUnit.Point, Value = score.TotalPoints, Description = "Runs" }
    };
}
