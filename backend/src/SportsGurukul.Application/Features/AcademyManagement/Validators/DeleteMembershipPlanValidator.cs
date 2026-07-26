using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteMembershipPlan;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class DeleteMembershipPlanValidator : AbstractValidator<DeleteMembershipPlanCommand>
{
    public DeleteMembershipPlanValidator()
    {
        RuleFor(x => x.MembershipId)
            .NotEmpty().WithMessage("Membership ID is required.");
    }
}
