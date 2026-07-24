namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachDocumentVersionDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    public string StoredFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Checksum { get; set; }
    public Guid? UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
