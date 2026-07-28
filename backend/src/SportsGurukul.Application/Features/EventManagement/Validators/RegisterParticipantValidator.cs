using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.RegisterParticipant;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class RegisterParticipantValidator : AbstractValidator<RegisterParticipantCommand>
{
    public RegisterParticipantValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");
    }
}
