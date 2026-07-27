using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentStage : BaseEntity
{
    public Guid TournamentId { get; set; }
    public string StageName { get; set; } = string.Empty;
    public StageType StageType { get; set; }
    public int StageOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCompleted { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public ICollection<TournamentRound> Rounds { get; set; } = new List<TournamentRound>();
}
