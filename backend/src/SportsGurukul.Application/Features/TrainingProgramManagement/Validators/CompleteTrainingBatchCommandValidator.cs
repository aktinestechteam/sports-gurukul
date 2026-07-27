using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CompleteTrainingBatch;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CompleteTrainingBatchCommandValidator : AbstractValidator<CompleteTrainingBatchCommand>
{
    public CompleteTrainingBatchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Batch ID is required.");
    }
}
