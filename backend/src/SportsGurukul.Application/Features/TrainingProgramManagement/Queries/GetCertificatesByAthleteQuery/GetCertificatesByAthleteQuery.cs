using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCertificatesByAthleteQuery;

public class GetCertificatesByAthleteQuery : IRequest<Result<IReadOnlyList<CertificateDto>>>
{
    public Guid AthleteId { get; set; }
}
