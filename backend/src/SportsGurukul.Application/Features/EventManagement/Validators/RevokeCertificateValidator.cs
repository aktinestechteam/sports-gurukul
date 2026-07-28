using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.RevokeCertificate;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class RevokeCertificateValidator : AbstractValidator<RevokeCertificateCommand>
{
    public RevokeCertificateValidator()
    {
        RuleFor(x => x.CertificateId)
            .NotEmpty().WithMessage("Certificate ID is required.");
    }
}
