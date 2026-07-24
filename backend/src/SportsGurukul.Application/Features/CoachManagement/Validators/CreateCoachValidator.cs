using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class CreateCoachValidator : AbstractValidator<CreateCoachCommand>
{
    public CreateCoachValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience must be non-negative.");

        RuleFor(x => x.Biography)
            .MaximumLength(2000).WithMessage("Biography must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Biography));

        RuleFor(x => x.CurrentOrganization)
            .MaximumLength(200).WithMessage("Current organization must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.CurrentOrganization));

        RuleFor(x => x.HighestQualification)
            .MaximumLength(200).WithMessage("Highest qualification must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.HighestQualification));

        RuleFor(x => x.PreferredLanguage)
            .MaximumLength(50).WithMessage("Preferred language must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PreferredLanguage));
    }
}
