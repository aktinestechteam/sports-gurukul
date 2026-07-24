using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AddCertification;

public class AddCertificationCommand : IRequest<Result<CertificationDto>>
{
    public Guid CoachId { get; set; }
    public string CertificationName { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertificateUrl { get; set; }
}
