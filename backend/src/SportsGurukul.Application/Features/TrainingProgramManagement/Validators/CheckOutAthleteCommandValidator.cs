using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckOutAthlete;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CheckOutAthleteCommandValidator : AbstractValidator<CheckOutAthleteCommand>
{
    public CheckOutAthleteCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
