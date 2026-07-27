using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckInAthlete;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CheckInAthleteCommandValidator : AbstractValidator<CheckInAthleteCommand>
{
    public CheckInAthleteCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
