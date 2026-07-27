using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class LiveScoreEvent
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public ScoringUnit Unit { get; set; }
    public int Points { get; set; }
    public int PeriodNumber { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsUndo { get; set; }
    public Guid? UndoEventId { get; set; }
}
