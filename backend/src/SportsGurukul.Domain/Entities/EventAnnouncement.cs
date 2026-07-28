using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventAnnouncement : BaseEntity
{
    public Guid EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid? PublishedBy { get; set; }
    public bool SendNotification { get; set; }
    public string? Priority { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
}
