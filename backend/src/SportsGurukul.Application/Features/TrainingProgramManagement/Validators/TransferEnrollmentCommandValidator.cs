using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.TransferEnrollment;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class TransferEnrollmentCommandValidator : AbstractValidator<TransferEnrollmentCommand>
{
    public TransferEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        RuleFor(x => x.TargetBatchId)
            .NotEmpty().WithMessage("Target batch ID is required.");
    }
}
