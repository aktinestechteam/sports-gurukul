using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveCoach;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RemoveCoachValidator : AbstractValidator<RemoveCoachCommand>
{
    public RemoveCoachValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
