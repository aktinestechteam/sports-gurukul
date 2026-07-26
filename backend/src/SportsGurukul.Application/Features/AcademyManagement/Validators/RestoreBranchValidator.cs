using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreBranch;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RestoreBranchValidator : AbstractValidator<RestoreBranchCommand>
{
    public RestoreBranchValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch ID is required.");
    }
}
