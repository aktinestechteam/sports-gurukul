using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.MedalTable;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class MedalTableQueryValidator : AbstractValidator<MedalTableQuery>
{
    public MedalTableQueryValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");
    }
}
