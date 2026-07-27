using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.AssignCoachToBatch;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class AssignCoachToBatchCommandValidator : AbstractValidator<AssignCoachToBatchCommand>
{
    public AssignCoachToBatchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Batch ID is required.");

        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("New coach ID is required.");
    }
}
