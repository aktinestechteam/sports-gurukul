using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AcademySocialLink : BaseEntity
{
    public Guid AcademyId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public Academy Academy { get; set; } = null!;
}
