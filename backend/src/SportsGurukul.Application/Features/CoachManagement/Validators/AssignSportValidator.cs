using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.AssignSport;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class AssignSportValidator : AbstractValidator<AssignSportCommand>
{
    public AssignSportValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");
    }
}
