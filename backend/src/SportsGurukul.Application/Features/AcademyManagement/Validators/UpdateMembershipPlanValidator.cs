using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateMembershipPlan;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class UpdateMembershipPlanValidator : AbstractValidator<UpdateMembershipPlanCommand>
{
    public UpdateMembershipPlanValidator()
    {
        RuleFor(x => x.MembershipId)
            .NotEmpty().WithMessage("Membership ID is required.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.MembershipName)
            .MaximumLength(200).WithMessage("Membership name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MembershipName));

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.")
            .When(x => x.Duration.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Benefits)
            .MaximumLength(2000).WithMessage("Benefits must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Benefits));
    }
}
