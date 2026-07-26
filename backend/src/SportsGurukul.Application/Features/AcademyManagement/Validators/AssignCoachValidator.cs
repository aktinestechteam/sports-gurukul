using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.AssignCoach;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class AssignCoachValidator : AbstractValidator<AssignCoachCommand>
{
    public AssignCoachValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
