using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAchievement;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class DeleteAchievementValidator : AbstractValidator<DeleteAchievementCommand>
{
    public DeleteAchievementValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.AchievementId)
            .NotEmpty().WithMessage("Achievement ID is required.");
    }
}
