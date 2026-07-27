namespace SportsGurukul.Platform.Competition.Models.Enums;

public enum CompetitionFormat
{
    SingleElimination = 0,
    DoubleElimination = 1,
    RoundRobin = 2,
    SwissSystem = 3,
    League = 4,
    HybridTournament = 5,
    GroupStageKnockout = 6
}

public enum SeedingStrategy
{
    Random = 0,
    RankingBased = 1,
    Manual = 2,
    Regional = 3,
    AcademyBased = 4,
    BalancedDraw = 5
}

public enum MatchStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Walkover = 3,
    Forfeit = 4,
    Cancelled = 5,
    Postponed = 6,
    Disqualified = 7
}

public enum BracketType
{
    Main = 0,
    Consolation = 1,
    ThirdPlace = 2,
    Winners = 3,
    Losers = 4,
    GrandFinal = 5
}

public enum AdvancementReason
{
    Win = 0,
    Walkover = 1,
    Forfeit = 2,
    Bye = 3,
    Default = 4,
    Tiebreak = 5
}

public enum RoundType
{
    Group = 0,
    RoundRobin = 1,
    KnockoutRound = 2,
    SemiFinal = 3,
    Final = 4,
    ThirdPlace = 5,
    SwissRound = 6,
    LeagueMatchday = 7
}

public enum RankingTiebreaker
{
    HeadToHead = 0,
    GoalDifference = 1,
    GoalsScored = 2,
    PointsDifference = 3,
    SetsWon = 4,
    GamesWon = 5,
    Wins = 6,
    CoinToss = 7
}
