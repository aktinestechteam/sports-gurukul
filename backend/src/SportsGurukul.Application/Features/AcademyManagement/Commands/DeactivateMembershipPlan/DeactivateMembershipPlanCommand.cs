using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeactivateMembershipPlan;

public class DeactivateMembershipPlanCommand : IRequest<Result<MembershipPlanDto>>
{
    public Guid MembershipId { get; set; }
}
