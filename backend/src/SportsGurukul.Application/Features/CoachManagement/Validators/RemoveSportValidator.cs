using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.RemoveSport;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class RemoveSportValidator : AbstractValidator<RemoveSportCommand>
{
    public RemoveSportValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");
    }
}
