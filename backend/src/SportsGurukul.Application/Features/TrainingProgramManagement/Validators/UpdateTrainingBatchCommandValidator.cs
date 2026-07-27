using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.UpdateTrainingBatch;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class UpdateTrainingBatchCommandValidator : AbstractValidator<UpdateTrainingBatchCommand>
{
    public UpdateTrainingBatchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Batch ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be on or after start date.")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.MaximumSeats)
            .InclusiveBetween(1, 500).WithMessage("Maximum seats must be between 1 and 500.");
    }
}
