using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCertification;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class UpdateCertificationValidator : AbstractValidator<UpdateCertificationCommand>
{
    public UpdateCertificationValidator()
    {
        RuleFor(x => x.CertificationId)
            .NotEmpty().WithMessage("Certification ID is required.");

        RuleFor(x => x.CertificationName)
            .MaximumLength(200).WithMessage("Certification name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.CertificationName));

        RuleFor(x => x.IssuingAuthority)
            .MaximumLength(200).WithMessage("Issuing authority must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.IssuingAuthority));

        RuleFor(x => x.CertificateNumber)
            .MaximumLength(100).WithMessage("Certificate number must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.CertificateNumber));

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.IssueDate).WithMessage("Expiry date must be after issue date.")
            .When(x => x.ExpiryDate.HasValue && x.IssueDate.HasValue);
    }
}
