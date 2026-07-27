using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentBracket : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? DivisionId { get; set; }
    public string BracketName { get; set; } = string.Empty;
    public TournamentType BracketType { get; set; }
    public int? TotalRounds { get; set; }
    public bool IsCompleted { get; set; }
    public string? BracketData { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentDivision? Division { get; set; }
}
