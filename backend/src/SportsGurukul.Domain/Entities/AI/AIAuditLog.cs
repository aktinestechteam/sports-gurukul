using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIAuditLog : BaseEntity
{
    public Guid? EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public AuditEventType EventType { get; set; }
    public AuditSeverity Severity { get; set; } = AuditSeverity.Info;
    public string? Action { get; set; }
    public string? ActorId { get; set; }
    public string? ActorType { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? PreviousState { get; set; }
    public string? NewState { get; set; }
    public string? Message { get; set; }
    public string? Metadata { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
