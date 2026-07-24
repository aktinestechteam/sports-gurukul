using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeactivateCoach;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class DeactivateCoachValidator : AbstractValidator<DeactivateCoachCommand>
{
    public DeactivateCoachValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
