using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.RescheduleTrainingSession;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class RescheduleTrainingSessionCommandValidator : AbstractValidator<RescheduleTrainingSessionCommand>
{
    public RescheduleTrainingSessionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.SessionDate)
            .NotEmpty().WithMessage("Session date is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");
    }
}
