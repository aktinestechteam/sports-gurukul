namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachDocumentDto
{
    public Guid Id { get; set; }
    public Guid CoachId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Checksum { get; set; }
    public int Version { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? UploadedBy { get; set; }
    public DateTime UploadedOn { get; set; }
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsPublic { get; set; }
    public string? Remarks { get; set; }
    public string? RejectionReason { get; set; }
    public string? DownloadUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<CoachDocumentVersionDto> Versions { get; set; } = [];
    public IReadOnlyList<CoachDocumentAuditDto> AuditTrail { get; set; } = [];
}
