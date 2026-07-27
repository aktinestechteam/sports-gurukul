using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentDocument : BaseEntity
{
    public Guid TournamentId { get; set; }
    public TournamentDocumentType DocumentType { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public bool IsPublished { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
}
