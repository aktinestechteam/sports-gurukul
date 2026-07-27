using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TrainingEnrollment : BaseEntity
{
    public Guid BatchId { get; set; }
    public Guid AthleteId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public byte[] RowVersion { get; set; } = [];

    public TrainingBatch Batch { get; set; } = null!;
    public Athlete Athlete { get; set; } = null!;
    public TrainingProgress? Progress { get; set; }
    public ICollection<TrainingCertificate> Certificates { get; set; } = new List<TrainingCertificate>();
}
