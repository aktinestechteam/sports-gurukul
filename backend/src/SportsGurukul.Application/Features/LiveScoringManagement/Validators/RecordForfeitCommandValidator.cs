using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordForfeit;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class RecordForfeitCommandValidator : AbstractValidator<RecordForfeitCommand>
{
    public RecordForfeitCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");

        RuleFor(x => x.WinnerId)
            .NotEmpty().WithMessage("Winner ID is required.");

        RuleFor(x => x.WinnerName)
            .NotEmpty().WithMessage("Winner name is required.")
            .MaximumLength(200).WithMessage("Winner name must not exceed 200 characters.");
    }
}
