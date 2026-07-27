using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CompleteMatch;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class CompleteMatchCommandValidator : AbstractValidator<CompleteMatchCommand>
{
    public CompleteMatchCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
