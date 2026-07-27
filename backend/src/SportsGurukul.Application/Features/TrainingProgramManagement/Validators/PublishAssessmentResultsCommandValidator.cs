using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.PublishAssessmentResults;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class PublishAssessmentResultsCommandValidator : AbstractValidator<PublishAssessmentResultsCommand>
{
    public PublishAssessmentResultsCommandValidator()
    {
        RuleFor(x => x.AssessmentId)
            .NotEmpty().WithMessage("Assessment ID is required.");
    }
}
