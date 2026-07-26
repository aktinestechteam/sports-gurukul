using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteBranch;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class DeleteBranchValidator : AbstractValidator<DeleteBranchCommand>
{
    public DeleteBranchValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch ID is required.");
    }
}
