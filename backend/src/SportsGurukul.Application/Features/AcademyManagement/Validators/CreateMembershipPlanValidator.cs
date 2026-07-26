using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class CreateMembershipPlanValidator : AbstractValidator<CreateMembershipPlanCommand>
{
    public CreateMembershipPlanValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.MembershipName)
            .NotEmpty().WithMessage("Membership name is required.")
            .MaximumLength(200).WithMessage("Membership name must not exceed 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Benefits)
            .MaximumLength(2000).WithMessage("Benefits must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Benefits));
    }
}
