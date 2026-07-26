using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveAthlete;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RemoveAthleteValidator : AbstractValidator<RemoveAthleteCommand>
{
    public RemoveAthleteValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
