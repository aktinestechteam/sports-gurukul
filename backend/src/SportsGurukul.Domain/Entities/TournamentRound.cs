using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentRound : BaseEntity
{
    public Guid TournamentStageId { get; set; }
    public int RoundNumber { get; set; }
    public string? RoundName { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public bool IsCompleted { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public TournamentStage TournamentStage { get; set; } = null!;
    public ICollection<TournamentMatch> Matches { get; set; } = new List<TournamentMatch>();
}
