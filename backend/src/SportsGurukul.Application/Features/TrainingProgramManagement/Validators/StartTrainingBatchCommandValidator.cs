using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.StartTrainingBatch;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class StartTrainingBatchCommandValidator : AbstractValidator<StartTrainingBatchCommand>
{
    public StartTrainingBatchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Batch ID is required.");
    }
}
