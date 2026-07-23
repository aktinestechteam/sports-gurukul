using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AssignSport;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class AssignSportValidator : AbstractValidator<AssignSportCommand>
{
    public AssignSportValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");
    }
}
