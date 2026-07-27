using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.SubmitAssessmentResult;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class SubmitAssessmentResultCommandValidator : AbstractValidator<SubmitAssessmentResultCommand>
{
    public SubmitAssessmentResultCommandValidator()
    {
        RuleFor(x => x.AssessmentId)
            .NotEmpty().WithMessage("Assessment ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Score must be greater than or equal to 0.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.");
    }
}
