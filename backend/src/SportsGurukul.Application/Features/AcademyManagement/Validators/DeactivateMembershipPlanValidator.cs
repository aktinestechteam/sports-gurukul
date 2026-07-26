using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeactivateMembershipPlan;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class DeactivateMembershipPlanValidator : AbstractValidator<DeactivateMembershipPlanCommand>
{
    public DeactivateMembershipPlanValidator()
    {
        RuleFor(x => x.MembershipId)
            .NotEmpty().WithMessage("Membership ID is required.");
    }
}
