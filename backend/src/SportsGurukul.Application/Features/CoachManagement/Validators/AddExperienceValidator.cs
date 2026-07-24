using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddExperience;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class AddExperienceValidator : AbstractValidator<AddExperienceCommand>
{
    public AddExperienceValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.Organization)
            .NotEmpty().WithMessage("Organization is required.")
            .MaximumLength(200).WithMessage("Organization must not exceed 200 characters.");

        RuleFor(x => x.Role)
            .MaximumLength(200).WithMessage("Role must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Role));

        RuleFor(x => x.Sport)
            .MaximumLength(100).WithMessage("Sport must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Sport));

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
