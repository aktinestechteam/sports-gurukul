using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateRankings;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class GenerateRankingsCommandValidator : AbstractValidator<GenerateRankingsCommand>
{
    public GenerateRankingsCommandValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");
    }
}
