using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class AcademyVerification : BaseEntity
{
    public Guid AcademyId { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? Remarks { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Academy Academy { get; set; } = null!;
}
