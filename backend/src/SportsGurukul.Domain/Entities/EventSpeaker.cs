using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventSpeaker : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CoachId { get; set; }
    public string SpeakerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Organization { get; set; }
    public EventSpeakerRole Role { get; set; } = EventSpeakerRole.Speaker;
    public string? Topic { get; set; }
    public int DisplayOrder { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public User? User { get; set; }
    public Coach? Coach { get; set; }
}
