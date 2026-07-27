using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services.Providers;

public class FootballSportRuleProvider : ISportRuleProvider
{
    public string SportCode => "FOOTBALL";
    public string SportName => "Football";

    public SportScoringConfig GetScoringConfig() => new()
    {
        SportCode = SportCode,
        SportName = SportName,
        PrimaryUnit = ScoringUnit.Point,
        SupportedUnits = new List<ScoringUnit> { ScoringUnit.Point },
        PointsForWin = 3,
        PointsForDraw = 1,
        PointsForLoss = 0,
        AllowsDraws = true,
        HasOvertime = false,
        HasPenaltyShootout = true,
        TieBreakers = new List<string> { "Head-to-Head", "Goal Difference", "Goals Scored" }
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

    public IReadOnlyList<string> GetTieBreakers() => new List<string> { "Head-to-Head", "Goal Difference", "Goals Scored" };

    public MatchScore CreateEmptyScore() => new();

    public bool IsMatchComplete(LiveMatch match) => match.ScoreEvents.Any(e => !e.IsUndo);

    public List<ScoringBreakdown> GetScoreBreakdown(MatchScore score) => new()
    {
        new ScoringBreakdown { Unit = ScoringUnit.Point, Value = score.TotalPoints, Description = "Goals" }
    };
}
