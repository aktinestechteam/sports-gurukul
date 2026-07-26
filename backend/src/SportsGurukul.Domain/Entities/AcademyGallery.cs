using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AcademyGallery : BaseEntity
{
    public Guid AcademyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsFeatured { get; set; }

    public Academy Academy { get; set; } = null!;
}
