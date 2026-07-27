using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.CompleteMatch;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class CompleteMatchCommandValidator : AbstractValidator<CompleteMatchCommand>
{
    public CompleteMatchCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
