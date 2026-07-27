using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.CreateAssessment;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CreateAssessmentCommandValidator : AbstractValidator<CreateAssessmentCommand>
{
    public CreateAssessmentCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.AssessmentType)
            .Must(type => Enum.TryParse<AssessmentType>(type, true, out _))
            .WithMessage("Invalid assessment type.");

        RuleFor(x => x.AssessmentName)
            .NotEmpty().WithMessage("Assessment name is required.")
            .MaximumLength(200).WithMessage("Assessment name must not exceed 200 characters.");

        RuleFor(x => x.MaximumScore)
            .GreaterThan(0).WithMessage("Maximum score must be greater than 0.");

        RuleFor(x => x.PassingScore)
            .GreaterThanOrEqualTo(0).WithMessage("Passing score must be greater than or equal to 0.")
            .LessThanOrEqualTo(x => x.MaximumScore).WithMessage("Passing score must not exceed maximum score.");
    }
}
