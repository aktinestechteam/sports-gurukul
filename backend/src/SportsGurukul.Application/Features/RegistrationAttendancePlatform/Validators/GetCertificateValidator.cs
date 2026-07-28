using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetCertificate;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class GetCertificateValidator : AbstractValidator<GetCertificateQuery>
{
    public GetCertificateValidator()
    {
        RuleFor(x => x.CertificateNumber)
            .NotEmpty().WithMessage("Certificate number is required.");
    }
}
