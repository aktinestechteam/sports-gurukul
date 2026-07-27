using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentGallery : BaseEntity
{
    public Guid TournamentId { get; set; }
    public TournamentGalleryMediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
    public string? ThumbnailUrl { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
}
