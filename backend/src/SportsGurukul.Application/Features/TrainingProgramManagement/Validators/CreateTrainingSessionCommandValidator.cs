using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CreateTrainingSession;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CreateTrainingSessionCommandValidator : AbstractValidator<CreateTrainingSessionCommand>
{
    public CreateTrainingSessionCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty().WithMessage("Batch ID is required.");

        RuleFor(x => x.SessionTitle)
            .NotEmpty().WithMessage("Session title is required.")
            .MaximumLength(200).WithMessage("Session title must not exceed 200 characters.");

        RuleFor(x => x.SessionType)
            .IsInEnum().WithMessage("Invalid session type.");

        RuleFor(x => x.SessionDate)
            .NotEmpty().WithMessage("Session date is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");
    }
}
