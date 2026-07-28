using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RevokeCertificate;

public class RevokeCertificateCommand : IRequest<Result<CertificateDto>>
{
    public Guid CertificateId { get; set; }
    public string? Reason { get; set; }
}
