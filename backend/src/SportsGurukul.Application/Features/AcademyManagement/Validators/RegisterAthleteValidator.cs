using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RegisterAthlete;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RegisterAthleteValidator : AbstractValidator<RegisterAthleteCommand>
{
    public RegisterAthleteValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
