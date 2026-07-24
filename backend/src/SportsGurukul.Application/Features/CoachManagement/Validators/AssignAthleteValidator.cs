using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.AssignAthlete;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class AssignAthleteValidator : AbstractValidator<AssignAthleteCommand>
{
    public AssignAthleteValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
