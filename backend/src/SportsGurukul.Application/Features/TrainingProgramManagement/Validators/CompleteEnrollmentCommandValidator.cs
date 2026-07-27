using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CompleteEnrollment;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CompleteEnrollmentCommandValidator : AbstractValidator<CompleteEnrollmentCommand>
{
    public CompleteEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");
    }
}
