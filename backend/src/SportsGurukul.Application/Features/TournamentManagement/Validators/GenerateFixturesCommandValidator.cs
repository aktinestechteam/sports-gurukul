using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateFixtures;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class GenerateFixturesCommandValidator : AbstractValidator<GenerateFixturesCommand>
{
    public GenerateFixturesCommandValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");
    }
}
