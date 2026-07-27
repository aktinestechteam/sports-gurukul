using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CreateTrainingBatch;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CreateTrainingBatchCommandValidator : AbstractValidator<CreateTrainingBatchCommand>
{
    public CreateTrainingBatchCommandValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");

        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(date => date > DateTime.UtcNow).WithMessage("Start date must be in the future.");

        RuleFor(x => x.MaximumSeats)
            .InclusiveBetween(1, 500).WithMessage("Maximum seats must be between 1 and 500.");
    }
}
