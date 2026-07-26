using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetAssignedCoaches;

public class GetAssignedCoachesQuery : IRequest<Result<IReadOnlyList<AcademyCoachSummaryDto>>>
{
    public Guid AcademyId { get; set; }
}
