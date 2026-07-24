using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCertification;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class DeleteCertificationValidator : AbstractValidator<DeleteCertificationCommand>
{
    public DeleteCertificationValidator()
    {
        RuleFor(x => x.CertificationId)
            .NotEmpty().WithMessage("Certification ID is required.");
    }
}
