using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateExperience;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class UpdateExperienceValidator : AbstractValidator<UpdateExperienceCommand>
{
    public UpdateExperienceValidator()
    {
        RuleFor(x => x.ExperienceId)
            .NotEmpty().WithMessage("Experience ID is required.");

        RuleFor(x => x.Organization)
            .MaximumLength(200).WithMessage("Organization must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Organization));

        RuleFor(x => x.Role)
            .MaximumLength(200).WithMessage("Role must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Role));

        RuleFor(x => x.Sport)
            .MaximumLength(100).WithMessage("Sport must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Sport));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
