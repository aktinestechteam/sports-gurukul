using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachCertifications;

public class GetCoachCertificationsQuery : IRequest<Result<IReadOnlyList<CertificationDto>>>
{
    public Guid CoachId { get; set; }
}
