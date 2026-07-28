using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventCoach : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid CoachId { get; set; }
    public string? Role { get; set; }
    public string? Responsibility { get; set; }
    public bool IsLeadCoach { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public Coach Coach { get; set; } = null!;
}
