using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.IssueCertificate;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class IssueCertificateValidator : AbstractValidator<IssueCertificateCommand>
{
    public IssueCertificateValidator()
    {
        RuleFor(x => x.CertificateId)
            .NotEmpty().WithMessage("Certificate ID is required.");
    }
}
