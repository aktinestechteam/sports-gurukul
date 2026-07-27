using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.LiveScore;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class LiveScoreQueryValidator : AbstractValidator<LiveScoreQuery>
{
    public LiveScoreQueryValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
