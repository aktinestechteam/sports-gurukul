using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AssessmentResult : BaseEntity
{
    public Guid AssessmentId { get; set; }
    public Guid AthleteId { get; set; }
    public decimal Score { get; set; }
    public bool IsPassed { get; set; }
    public string? Remarks { get; set; }
    public DateTime AssessedAt { get; set; }

    public TrainingAssessment Assessment { get; set; } = null!;
    public Athlete Athlete { get; set; } = null!;
}
