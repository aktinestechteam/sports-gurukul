namespace SportsGurukul.Application.Features.DocumentManagement.DTOs;

public class DocumentDownloadDto
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Stream Content { get; set; } = Stream.Null;
}
