using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.PauseMatch;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class PauseMatchCommandValidator : AbstractValidator<PauseMatchCommand>
{
    public PauseMatchCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
