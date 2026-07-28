using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.MarkAttendance;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class MarkAttendanceValidator : AbstractValidator<MarkAttendanceCommand>
{
    public MarkAttendanceValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
    }
}
