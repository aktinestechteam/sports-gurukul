using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventCertificate : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid ParticipantId { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string? CertificateType { get; set; }
    public DateTime IssuedDate { get; set; }
    public string? IssuedBy { get; set; }
    public string? DocumentUrl { get; set; }
    public bool IsPrinted { get; set; }
    public bool IsSent { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public EventParticipant Participant { get; set; } = null!;
}
