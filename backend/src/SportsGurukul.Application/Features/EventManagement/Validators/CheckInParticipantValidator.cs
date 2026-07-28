using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class CheckInParticipantValidator : AbstractValidator<CheckInParticipantCommand>
{
    public CheckInParticipantValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
    }
}
