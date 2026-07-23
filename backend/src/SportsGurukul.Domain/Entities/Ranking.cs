using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class Ranking : BaseEntity
{
    public Guid AthleteId { get; set; }
    public string? CurrentRank { get; set; }
    public string? StateRank { get; set; }
    public string? NationalRank { get; set; }
    public string? InternationalRank { get; set; }
    public string? RankingAuthority { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Athlete Athlete { get; set; } = null!;
}
