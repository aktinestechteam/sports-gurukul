using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AcademySport : BaseEntity
{
    public Guid AcademyId { get; set; }
    public Guid SportId { get; set; }
    public bool IsPrimarySport { get; set; }
    public DateTime JoinedDate { get; set; }

    public Academy Academy { get; set; } = null!;
    public Sport Sport { get; set; } = null!;
}
