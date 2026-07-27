using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCertificateByIdQuery;

public class GetCertificateByIdQuery : IRequest<Result<CertificateDto>>
{
    public Guid Id { get; set; }
}
