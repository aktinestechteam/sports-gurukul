using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddCertification;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class AddCertificationValidator : AbstractValidator<AddCertificationCommand>
{
    public AddCertificationValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.CertificationName)
            .NotEmpty().WithMessage("Certification name is required.")
            .MaximumLength(200).WithMessage("Certification name must not exceed 200 characters.");

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
