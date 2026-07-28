using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RevokeCertificate;

public class RevokeCertificateCommand : IRequest<Result<PlatformCertificateDto>>
{
    public Guid CertificateId { get; set; }
    public string? Reason { get; set; }
    public string? RevokedBy { get; set; }
}
