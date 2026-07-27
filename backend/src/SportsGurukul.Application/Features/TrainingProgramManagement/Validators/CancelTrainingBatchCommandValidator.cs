using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CancelTrainingBatch;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CancelTrainingBatchCommandValidator : AbstractValidator<CancelTrainingBatchCommand>
{
    public CancelTrainingBatchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Batch ID is required.");
    }
}
