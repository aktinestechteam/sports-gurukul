using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CancelEnrollment;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CancelEnrollmentCommandValidator : AbstractValidator<CancelEnrollmentCommand>
{
    public CancelEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");
    }
}
