using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteMembershipPlan;

public class DeleteMembershipPlanCommand : IRequest<Result<Unit>>
{
    public Guid MembershipId { get; set; }
}
