using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Notification;

public class TemplateVariable : BaseEntity
{
    public Guid TemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string DataType { get; set; } = "string";

    public NotificationTemplate Template { get; set; } = null!;
}
