using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.IssueCertificate;

public class IssueCertificateCommand : IRequest<Result<CertificateDto>>
{
    public Guid EventId { get; set; }
    public Guid ParticipantId { get; set; }
    public string? CertificateType { get; set; }
    public string? Notes { get; set; }
}
