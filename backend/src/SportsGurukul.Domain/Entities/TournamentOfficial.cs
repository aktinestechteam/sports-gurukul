using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentOfficial : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? CoachId { get; set; }
    public string OfficialName { get; set; } = string.Empty;
    public TournamentOfficialRole Role { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public Coach? Coach { get; set; }
}
