using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TrainingBatch : BaseEntity
{
    public Guid ProgramId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public Guid CoachId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaximumSeats { get; set; }
    public BatchStatus Status { get; set; } = BatchStatus.Active;
    public byte[] RowVersion { get; set; } = [];

    public TrainingProgram Program { get; set; } = null!;
    public Coach Coach { get; set; } = null!;
    public AcademyBranch Branch { get; set; } = null!;
    public ICollection<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
    public ICollection<TrainingEnrollment> Enrollments { get; set; } = new List<TrainingEnrollment>();
}
