using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.GenerateLeaderboard;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class GenerateLeaderboardCommandValidator : AbstractValidator<GenerateLeaderboardCommand>
{
    public GenerateLeaderboardCommandValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");
    }
}
