using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates;

public class GenerateCertificatesCommand : IRequest<Result<List<CertificateDto>>>
{
    public Guid EventId { get; set; }
}
