namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CertificationDto
{
    public Guid Id { get; set; }
    public string CertificationName { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public string? CertificateUrl { get; set; }
    public bool IsExpired { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
