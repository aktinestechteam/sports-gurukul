using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.CompleteMilestone;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CompleteMilestoneCommandValidator : AbstractValidator<CompleteMilestoneCommand>
{
    public CompleteMilestoneCommandValidator()
    {
        RuleFor(x => x.MilestoneId)
            .NotEmpty().WithMessage("Milestone ID is required.");
    }
}
