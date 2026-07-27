using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateScore;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class UpdateScoreCommandValidator : AbstractValidator<UpdateScoreCommand>
{
    public UpdateScoreCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");

        RuleFor(x => x.HomeScore)
            .GreaterThanOrEqualTo(0).WithMessage("Home score cannot be negative.");

        RuleFor(x => x.AwayScore)
            .GreaterThanOrEqualTo(0).WithMessage("Away score cannot be negative.");
    }
}
