using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TrainingAssessment : BaseEntity
{
    public Guid SessionId { get; set; }
    public AssessmentType AssessmentType { get; set; } = AssessmentType.SkillTest;
    public string AssessmentName { get; set; } = string.Empty;
    public decimal MaximumScore { get; set; }
    public decimal PassingScore { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public TrainingSession Session { get; set; } = null!;
    public ICollection<AssessmentResult> Results { get; set; } = new List<AssessmentResult>();
}
