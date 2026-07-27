using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.PublishResults;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class PublishResultsCommandValidator : AbstractValidator<PublishResultsCommand>
{
    public PublishResultsCommandValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");

        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
