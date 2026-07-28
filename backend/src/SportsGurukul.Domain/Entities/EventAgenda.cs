using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventAgenda : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? SessionId { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SpeakerName { get; set; }
    public string? Location { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public EventSession? Session { get; set; }
}
