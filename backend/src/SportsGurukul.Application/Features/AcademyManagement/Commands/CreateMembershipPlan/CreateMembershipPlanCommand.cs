using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;

public class CreateMembershipPlanCommand : IRequest<Result<MembershipPlanDto>>
{
    public Guid AcademyId { get; set; }
    public string MembershipName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string? Benefits { get; set; }
}
