using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AcademyView : BaseEntity
{
    public Guid AcademyId { get; set; }
    public Guid? ViewedByUserId { get; set; }
    public DateTime ViewedAt { get; set; }
    public string Source { get; set; } = string.Empty;

    public Academy Academy { get; set; } = null!;
}
