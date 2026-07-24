using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoach;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class DeleteCoachValidator : AbstractValidator<DeleteCoachCommand>
{
    public DeleteCoachValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
