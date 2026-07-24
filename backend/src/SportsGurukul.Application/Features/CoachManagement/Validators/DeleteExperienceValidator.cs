using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteExperience;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class DeleteExperienceValidator : AbstractValidator<DeleteExperienceCommand>
{
    public DeleteExperienceValidator()
    {
        RuleFor(x => x.ExperienceId)
            .NotEmpty().WithMessage("Experience ID is required.");
    }
}
