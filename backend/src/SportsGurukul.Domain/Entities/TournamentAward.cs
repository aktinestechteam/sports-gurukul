using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentAward : BaseEntity
{
    public Guid TournamentId { get; set; }
    public TournamentAwardType AwardType { get; set; }
    public string AwardName { get; set; } = string.Empty;
    public Guid? ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? Description { get; set; }
    public decimal? PrizeMoney { get; set; }
    public string? CertificateUrl { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentParticipant? Participant { get; set; }
    public TournamentTeam? Team { get; set; }
}
