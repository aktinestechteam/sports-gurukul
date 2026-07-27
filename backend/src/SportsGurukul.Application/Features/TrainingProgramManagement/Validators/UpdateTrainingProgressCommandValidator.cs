using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.UpdateTrainingProgress;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class UpdateTrainingProgressCommandValidator : AbstractValidator<UpdateTrainingProgressCommand>
{
    public UpdateTrainingProgressCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        RuleFor(x => x.CurrentLevel)
            .NotEmpty().WithMessage("Current level is required.")
            .MaximumLength(50).WithMessage("Current level must not exceed 50 characters.");

        RuleFor(x => x.CompletedPercentage)
            .InclusiveBetween(0, 100).WithMessage("Completed percentage must be between 0 and 100.");

        RuleFor(x => x.OverallRating)
            .InclusiveBetween(0, 5).WithMessage("Overall rating must be between 0 and 5.")
            .When(x => x.OverallRating.HasValue);
    }
}
