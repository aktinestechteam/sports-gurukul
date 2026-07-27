using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services.Providers;

public class AthleticsSportRuleProvider : ISportRuleProvider
{
    public string SportCode => "ATHLETICS";
    public string SportName => "Athletics";

    public SportScoringConfig GetScoringConfig() => new()
    {
        SportCode = SportCode,
        SportName = SportName,
        PrimaryUnit = ScoringUnit.Point,
        SupportedUnits = new List<ScoringUnit> { ScoringUnit.Point, ScoringUnit.Lap },
        PointsForWin = 0,
        PointsForDraw = 0,
        PointsForLoss = 0,
        AllowsDraws = false,
        TieBreakers = new List<string> { "Time", "Photo Finish" }
    };

    public int CalculateScore(LiveScoreEvent scoreEvent, MatchScore currentScore)
    {
        return currentScore.TotalPoints + scoreEvent.Points;
    }

    public bool DetermineWinner(LiveMatch match) => true;

    public Guid? DetermineWinner(Guid homeParticipantId, int homeScore, Guid awayParticipantId, int awayScore)
    {
        if (homeScore > awayScore) return homeParticipantId;
        if (awayScore > homeScore) return awayParticipantId;
        return homeParticipantId;
    }

    public IReadOnlyList<string> GetTieBreakers() => new List<string> { "Time", "Photo Finish" };

    public MatchScore CreateEmptyScore() => new();

    public bool IsMatchComplete(LiveMatch match) => match.ScoreEvents.Any(e => !e.IsUndo);

    public List<ScoringBreakdown> GetScoreBreakdown(MatchScore score) => new()
    {
        new ScoringBreakdown { Unit = ScoringUnit.Lap, Value = score.Laps, Description = "Laps/Distance" },
        new ScoringBreakdown { Unit = ScoringUnit.Point, Value = score.TotalPoints, Description = "Points" }
    };
}
