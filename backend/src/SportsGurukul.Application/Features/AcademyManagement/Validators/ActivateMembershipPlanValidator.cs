using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.ActivateMembershipPlan;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class ActivateMembershipPlanValidator : AbstractValidator<ActivateMembershipPlanCommand>
{
    public ActivateMembershipPlanValidator()
    {
        RuleFor(x => x.MembershipId)
            .NotEmpty().WithMessage("Membership ID is required.");
    }
}
