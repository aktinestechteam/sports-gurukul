using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EmergencyContact : BaseEntity
{
    public Guid AthleteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmergencyRelationship Relationship { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Athlete Athlete { get; set; } = null!;
}
