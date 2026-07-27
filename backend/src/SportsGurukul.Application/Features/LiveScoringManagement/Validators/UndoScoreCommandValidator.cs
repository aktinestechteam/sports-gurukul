using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.UndoScore;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class UndoScoreCommandValidator : AbstractValidator<UndoScoreCommand>
{
    public UndoScoreCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
