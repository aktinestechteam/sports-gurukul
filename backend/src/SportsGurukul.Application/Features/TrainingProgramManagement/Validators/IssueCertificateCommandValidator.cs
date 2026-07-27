using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.IssueCertificate;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class IssueCertificateCommandValidator : AbstractValidator<IssueCertificateCommand>
{
    public IssueCertificateCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        RuleFor(x => x.CertificateType)
            .Must(type => Enum.TryParse<CertificateType>(type, true, out _))
            .WithMessage("Invalid certificate type.");

        RuleFor(x => x.FileUrl)
            .MaximumLength(500).WithMessage("File URL must not exceed 500 characters.");
    }
}
