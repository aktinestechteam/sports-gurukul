using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class DocumentVersion : BaseEntity
{
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    public string StoredFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Checksum { get; set; }
    public Guid? UploadedBy { get; set; }

    public AthleteDocument Document { get; set; } = null!;
}
