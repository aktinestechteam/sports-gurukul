using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAthlete;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class UpdateAthleteValidator : AbstractValidator<UpdateAthleteCommand>
{
    public UpdateAthleteValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.ExperienceYears)
            .GreaterThanOrEqualTo(0).WithMessage("Experience years must be non-negative.")
            .When(x => x.ExperienceYears.HasValue);

        RuleFor(x => x.Height)
            .MaximumLength(20).WithMessage("Height must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Height));

        RuleFor(x => x.Weight)
            .MaximumLength(20).WithMessage("Weight must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Weight));

        RuleFor(x => x.Biography)
            .MaximumLength(2000).WithMessage("Biography must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Biography));
    }
}
