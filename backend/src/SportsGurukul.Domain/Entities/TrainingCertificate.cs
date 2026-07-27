using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TrainingCertificate : BaseEntity
{
    public Guid EnrollmentId { get; set; }
    public CertificateType CertificateType { get; set; } = CertificateType.Completion;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? FileUrl { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public TrainingEnrollment Enrollment { get; set; } = null!;
}
