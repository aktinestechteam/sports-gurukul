using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.ActivateCoach;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class ActivateCoachValidator : AbstractValidator<ActivateCoachCommand>
{
    public ActivateCoachValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
