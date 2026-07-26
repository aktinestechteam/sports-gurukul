using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreAcademy;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RestoreAcademyValidator : AbstractValidator<RestoreAcademyCommand>
{
    public RestoreAcademyValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");
    }
}
