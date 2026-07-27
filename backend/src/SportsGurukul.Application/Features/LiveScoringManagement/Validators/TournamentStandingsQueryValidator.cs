using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.TournamentStandings;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class TournamentStandingsQueryValidator : AbstractValidator<TournamentStandingsQuery>
{
    public TournamentStandingsQueryValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");
    }
}
