using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachAcademy : BaseEntity
{
    public Guid CoachId { get; set; }
    public Guid AcademyId { get; set; }
    public DateTime AssignedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Coach Coach { get; set; } = null!;
    public Academy Academy { get; set; } = null!;
}
