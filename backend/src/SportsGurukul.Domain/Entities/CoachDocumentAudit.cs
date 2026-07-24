using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class CoachDocumentAudit : BaseEntity
{
    public Guid DocumentId { get; set; }
    public DocumentAuditAction Action { get; set; }
    public Guid? PerformedBy { get; set; }
    public DateTime PerformedOn { get; set; }
    public string? IpAddress { get; set; }
    public string? Details { get; set; }

    public CoachDocument Document { get; set; } = null!;
}
