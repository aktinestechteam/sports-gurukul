using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RevokeCertificate;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class RevokeCertificateValidator : AbstractValidator<RevokeCertificateCommand>
{
    public RevokeCertificateValidator()
    {
        RuleFor(x => x.CertificateId)
            .NotEmpty().WithMessage("Certificate ID is required.");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required for certificate revocation.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}
