using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetCertificate;

public class GetCertificateQuery : IRequest<Result<PlatformCertificateDto>>
{
    public string CertificateNumber { get; set; } = string.Empty;
}
