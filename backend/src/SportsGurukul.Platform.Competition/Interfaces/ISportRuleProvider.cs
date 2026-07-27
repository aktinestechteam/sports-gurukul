using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ISportRuleProvider
{
    string SportCode { get; }
    string SportName { get; }
    SportScoringConfig GetScoringConfig();
    int CalculateScore(LiveScoreEvent scoreEvent, MatchScore currentScore);
    bool DetermineWinner(LiveMatch match);
    Guid? DetermineWinner(Guid homeParticipantId, int homeScore, Guid awayParticipantId, int awayScore);
    IReadOnlyList<string> GetTieBreakers();
    MatchScore CreateEmptyScore();
    bool IsMatchComplete(LiveMatch match);
    List<ScoringBreakdown> GetScoreBreakdown(MatchScore score);
}
