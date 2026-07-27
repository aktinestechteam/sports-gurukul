using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.StartMatch;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class StartLiveMatchCommandValidator : AbstractValidator<StartLiveMatchCommand>
{
    public StartLiveMatchCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
