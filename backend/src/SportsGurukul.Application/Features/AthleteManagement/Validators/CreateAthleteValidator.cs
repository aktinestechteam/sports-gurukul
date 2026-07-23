using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class CreateAthleteValidator : AbstractValidator<CreateAthleteCommand>
{
    public CreateAthleteValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ExperienceYears)
            .GreaterThanOrEqualTo(0).WithMessage("Experience years must be non-negative.");

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
