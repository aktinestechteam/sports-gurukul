using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class AcademyMembership : BaseEntity
{
    public Guid AcademyId { get; set; }
    public string MembershipName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string? Benefits { get; set; }
    public AcademyMembershipStatus Status { get; set; } = AcademyMembershipStatus.Active;

    public Academy Academy { get; set; } = null!;
}
