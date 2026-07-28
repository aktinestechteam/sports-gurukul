using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.AssignSpeaker;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class AssignSpeakerValidator : AbstractValidator<AssignSpeakerCommand>
{
    public AssignSpeakerValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.SpeakerId)
            .NotEmpty().WithMessage("Speaker ID is required.");
    }
}
