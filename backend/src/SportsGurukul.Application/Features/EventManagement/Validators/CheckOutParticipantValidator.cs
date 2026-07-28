using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckOutParticipant;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class CheckOutParticipantValidator : AbstractValidator<CheckOutParticipantCommand>
{
    public CheckOutParticipantValidator()
    {
        RuleFor(x => x.AttendanceId)
            .NotEmpty().WithMessage("Attendance ID is required.");
    }
}
