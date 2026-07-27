using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services.Providers;

public class BadmintonSportRuleProvider : ISportRuleProvider
{
    public string SportCode => "BADMINTON";
    public string SportName => "Badminton";

    public SportScoringConfig GetScoringConfig() => new()
    {
        SportCode = SportCode,
        SportName = SportName,
        PrimaryUnit = ScoringUnit.Point,
        SupportedUnits = new List<ScoringUnit> { ScoringUnit.Point, ScoringUnit.Game, ScoringUnit.Set },
        PointsForWin = 0,
        PointsForDraw = 0,
        PointsForLoss = 0,
        HasSets = true,
        MaxSets = 3,
        SetsToWin = 2,
        GamesToWinSet = 21,
        AllowsDraws = false,
        HasTieBreak = true,
        TieBreakers = new List<string> { "Sets Won", "Games Won" }
    };

    public int CalculateScore(LiveScoreEvent scoreEvent, MatchScore currentScore)
    {
        return currentScore.TotalPoints + scoreEvent.Points;
    }

    public bool DetermineWinner(LiveMatch match) => match.HomeScore.Sets != match.AwayScore.Sets;

    public Guid? DetermineWinner(Guid homeParticipantId, int homeScore, Guid awayParticipantId, int awayScore)
    {
        if (homeScore > awayScore) return homeParticipantId;
        if (awayScore > homeScore) return awayParticipantId;
        return null;
    }

    public IReadOnlyList<string> GetTieBreakers() => new List<string> { "Sets Won", "Games Won" };

    public MatchScore CreateEmptyScore() => new();

    public bool IsMatchComplete(LiveMatch match)
    {
        var config = GetScoringConfig();
        return match.HomeScore.Sets >= config.SetsToWin || match.AwayScore.Sets >= config.SetsToWin;
    }

    public List<ScoringBreakdown> GetScoreBreakdown(MatchScore score) => new()
    {
        new ScoringBreakdown { Unit = ScoringUnit.Point, Value = score.TotalPoints, Description = "Points" },
        new ScoringBreakdown { Unit = ScoringUnit.Game, Value = score.Games, Description = "Games" },
        new ScoringBreakdown { Unit = ScoringUnit.Set, Value = score.Sets, Description = "Sets" }
    };
}
