using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.IssueCertificate;

public class IssueCertificateCommand : IRequest<Result<PlatformCertificateDto>>
{
    public Guid CertificateId { get; set; }
    public string? IssuedBy { get; set; }
    public string? DocumentUrl { get; set; }
}
