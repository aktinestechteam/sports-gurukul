using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteAcademy;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class DeleteAcademyValidator : AbstractValidator<DeleteAcademyCommand>
{
    public DeleteAcademyValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");
    }
}
