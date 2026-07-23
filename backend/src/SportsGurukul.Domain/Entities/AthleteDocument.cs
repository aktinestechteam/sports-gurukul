using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class AthleteDocument : BaseEntity
{
    public Guid AthleteId { get; set; }
    public DocumentCategory Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string StorageProvider { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Checksum { get; set; }
    public int Version { get; set; } = 1;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public Guid? UploadedBy { get; set; }
    public DateTime UploadedOn { get; set; }
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsPublic { get; set; }

    public Athlete Athlete { get; set; } = null!;
    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    public ICollection<DocumentAudit> AuditTrail { get; set; } = new List<DocumentAudit>();
}
