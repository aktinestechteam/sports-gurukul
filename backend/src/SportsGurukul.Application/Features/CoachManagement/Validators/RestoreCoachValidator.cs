using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoach;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class RestoreCoachValidator : AbstractValidator<RestoreCoachCommand>
{
    public RestoreCoachValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
