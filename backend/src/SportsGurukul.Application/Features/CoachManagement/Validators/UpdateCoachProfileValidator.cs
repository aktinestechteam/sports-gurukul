using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachProfile;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class UpdateCoachProfileValidator : AbstractValidator<UpdateCoachProfileCommand>
{
    public UpdateCoachProfileValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience must be non-negative.")
            .When(x => x.YearsOfExperience.HasValue);

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
