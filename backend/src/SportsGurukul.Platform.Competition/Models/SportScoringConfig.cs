using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class SportScoringConfig
{
    public string SportCode { get; set; } = string.Empty;
    public string SportName { get; set; } = string.Empty;
    public ScoringUnit PrimaryUnit { get; set; }
    public List<ScoringUnit> SupportedUnits { get; set; } = new();
    public int PointsForWin { get; set; } = 3;
    public int PointsForDraw { get; set; } = 1;
    public int PointsForLoss { get; set; } = 0;
    public bool HasSets { get; set; }
    public bool HasPeriods { get; set; }
    public bool HasQuarters { get; set; }
    public bool HasInnings { get; set; }
    public int MaxSets { get; set; }
    public int MaxPeriods { get; set; }
    public int MaxQuarters { get; set; }
    public int MaxInnings { get; set; }
    public int SetsToWin { get; set; }
    public int GamesToWinSet { get; set; }
    public bool AllowsDraws { get; set; }
    public bool HasOvertime { get; set; }
    public bool HasPenaltyShootout { get; set; }
    public bool HasTieBreak { get; set; }
    public List<string> TieBreakers { get; set; } = new();
}
