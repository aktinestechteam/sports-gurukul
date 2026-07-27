using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.ResumeMatch;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class ResumeMatchCommandValidator : AbstractValidator<ResumeMatchCommand>
{
    public ResumeMatchCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
