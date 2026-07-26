using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RejectAcademyVerification;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RejectAcademyVerificationValidator : AbstractValidator<RejectAcademyVerificationCommand>
{
    public RejectAcademyVerificationValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.Remarks)
            .NotEmpty().WithMessage("Remarks are required for rejection.")
            .MaximumLength(1000).WithMessage("Remarks must not exceed 1000 characters.");
    }
}
