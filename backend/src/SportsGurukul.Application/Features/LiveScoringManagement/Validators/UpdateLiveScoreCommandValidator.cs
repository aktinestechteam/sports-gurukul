using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.UpdateLiveScore;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class UpdateLiveScoreCommandValidator : AbstractValidator<UpdateLiveScoreCommand>
{
    public UpdateLiveScoreCommandValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");

        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0).WithMessage("Points cannot be negative.");

        RuleFor(x => x.PeriodNumber)
            .GreaterThan(0).WithMessage("Period number must be greater than 0.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
