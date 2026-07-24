using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.RemoveAthlete;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class RemoveAthleteValidator : AbstractValidator<RemoveAthleteCommand>
{
    public RemoveAthleteValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
