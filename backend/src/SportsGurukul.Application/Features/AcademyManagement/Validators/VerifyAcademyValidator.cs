using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.VerifyAcademy;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class VerifyAcademyValidator : AbstractValidator<VerifyAcademyCommand>
{
    public VerifyAcademyValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.Remarks)
            .MaximumLength(1000).WithMessage("Remarks must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Remarks));
    }
}
