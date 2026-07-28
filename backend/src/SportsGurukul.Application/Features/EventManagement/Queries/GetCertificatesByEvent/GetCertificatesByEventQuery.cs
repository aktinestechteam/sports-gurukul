using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetCertificatesByEvent;

public class GetCertificatesByEventQuery : IRequest<Result<List<CertificateDto>>>
{
    public Guid EventId { get; set; }
}
