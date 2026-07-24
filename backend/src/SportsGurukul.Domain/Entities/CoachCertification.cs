using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class CoachCertification : BaseEntity
{
    public Guid CoachId { get; set; }
    public string CertificationName { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public string? CertificateUrl { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Coach Coach { get; set; } = null!;
}
