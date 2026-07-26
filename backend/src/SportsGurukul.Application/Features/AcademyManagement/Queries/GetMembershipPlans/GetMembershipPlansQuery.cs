using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetMembershipPlans;

public class GetMembershipPlansQuery : IRequest<Result<IReadOnlyList<MembershipPlanDto>>>
{
    public Guid AcademyId { get; set; }
}
