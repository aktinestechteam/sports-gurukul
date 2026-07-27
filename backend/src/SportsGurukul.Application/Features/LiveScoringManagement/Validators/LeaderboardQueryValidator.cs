using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.Leaderboard;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class LeaderboardQueryValidator : AbstractValidator<LeaderboardQuery>
{
    public LeaderboardQueryValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");
    }
}
