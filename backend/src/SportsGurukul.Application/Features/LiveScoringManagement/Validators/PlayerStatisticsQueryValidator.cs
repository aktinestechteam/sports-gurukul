using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.PlayerStatistics;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class PlayerStatisticsQueryValidator : AbstractValidator<PlayerStatisticsQuery>
{
    public PlayerStatisticsQueryValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");

        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("Player ID is required.");
    }
}
