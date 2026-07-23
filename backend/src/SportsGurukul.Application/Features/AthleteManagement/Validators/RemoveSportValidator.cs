using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RemoveSport;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class RemoveSportValidator : AbstractValidator<RemoveSportCommand>
{
    public RemoveSportValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");
    }
}
