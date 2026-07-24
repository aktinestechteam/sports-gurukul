namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachDocumentDownloadDto
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Stream Content { get; set; } = Stream.Null;
}
