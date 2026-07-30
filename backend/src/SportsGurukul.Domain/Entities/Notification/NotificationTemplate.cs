using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int CurrentVersion { get; set; }

    public ICollection<TemplateVersion> Versions { get; set; } = new List<TemplateVersion>();
    public ICollection<TemplateVariable> Variables { get; set; } = new List<TemplateVariable>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
