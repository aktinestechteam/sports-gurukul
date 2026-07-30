using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Notification;

public class TemplateVersion : BaseEntity
{
    public Guid TemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string? ChangeNotes { get; set; }
    public DateTime PublishedAt { get; set; }

    public NotificationTemplate Template { get; set; } = null!;
}
