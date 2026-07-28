using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class GenerateCertificatesValidator : AbstractValidator<GenerateCertificatesCommand>
{
    public GenerateCertificatesValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");
    }
}
