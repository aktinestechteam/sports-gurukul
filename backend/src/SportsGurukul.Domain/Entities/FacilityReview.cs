using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class FacilityReview : BaseEntity
{
    public Guid FacilityId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public bool IsApproved { get; set; }

    public Facility Facility { get; set; } = null!;
}
