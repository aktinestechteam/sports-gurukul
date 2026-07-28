using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.AssignCoach;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class AssignCoachValidator : AbstractValidator<AssignCoachCommand>
{
    public AssignCoachValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
