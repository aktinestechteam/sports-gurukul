using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.IssueCertificate;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class IssueCertificateValidator : AbstractValidator<IssueCertificateCommand>
{
    public IssueCertificateValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
    }
}
