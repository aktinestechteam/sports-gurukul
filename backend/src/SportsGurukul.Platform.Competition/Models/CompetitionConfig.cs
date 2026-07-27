using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class CompetitionConfig
{
    public Guid TournamentId { get; set; }
    public CompetitionFormat Format { get; set; }
    public SeedingStrategy SeedingStrategy { get; set; } = SeedingStrategy.Random;
    public int TotalParticipants { get; set; }
    public int? GroupsCount { get; set; }
    public int? AdvancementPerGroup { get; set; }
    public bool HasThirdPlaceMatch { get; set; }
    public bool HasConsolationBracket { get; set; }
    public int? RoundsCount { get; set; }
    public int PointsForWin { get; set; } = 3;
    public int PointsForDraw { get; set; } = 1;
    public int PointsForLoss { get; set; } = 0;
    public bool UseHomeAway { get; set; }
    public List<RankingTiebreaker> Tiebreakers { get; set; } = [RankingTiebreaker.HeadToHead, RankingTiebreaker.GoalDifference, RankingTiebreaker.GoalsScored];
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}
