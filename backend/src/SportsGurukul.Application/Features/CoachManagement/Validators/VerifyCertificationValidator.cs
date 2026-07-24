using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCertification;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class VerifyCertificationValidator : AbstractValidator<VerifyCertificationCommand>
{
    public VerifyCertificationValidator()
    {
        RuleFor(x => x.CertificationId)
            .NotEmpty().WithMessage("Certification ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid verification status is required.");
    }
}
