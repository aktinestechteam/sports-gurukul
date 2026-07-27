using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class Bracket
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BracketType Type { get; set; } = BracketType.Main;
    public CompetitionFormat Format { get; set; }
    public List<BracketRound> Rounds { get; set; } = new();
    public List<CompetitionMatch> Matches { get; set; } = new();
}

public class BracketRound
{
    public int RoundNumber { get; set; }
    public string RoundName { get; set; } = string.Empty;
    public RoundType RoundType { get; set; }
    public List<CompetitionMatch> Matches { get; set; } = new();
}
