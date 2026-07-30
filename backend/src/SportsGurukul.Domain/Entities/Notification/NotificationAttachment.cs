using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationAttachment : BaseEntity
{
    public Guid NotificationId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StorageType { get; set; } = "local";
    public Guid? DocumentId { get; set; }

    public Notification Notification { get; set; } = null!;
}
