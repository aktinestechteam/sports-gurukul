using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.UpdateTrainingSession;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class UpdateTrainingSessionCommandValidator : AbstractValidator<UpdateTrainingSessionCommand>
{
    public UpdateTrainingSessionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Session ID is required.");

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
    }
}
