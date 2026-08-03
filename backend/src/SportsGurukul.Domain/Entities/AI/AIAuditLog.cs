using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIAuditLog : BaseEntity
{
    public Guid? ActorUserId { get; set; }
    public AIResourceOwnerType ActorType { get; set; }
    public AIAuditAction Action { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? DetailsJson { get; set; }
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public string? ChangedFieldsJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? CorrelationId { get; set; }
    public AIAuditSeverity Severity { get; set; } = AIAuditSeverity.Info;
    public byte[] RowVersion { get; set; } = [];
}
