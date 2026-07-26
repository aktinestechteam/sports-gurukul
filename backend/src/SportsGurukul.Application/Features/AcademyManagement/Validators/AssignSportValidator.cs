using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.AssignSport;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class AssignSportValidator : AbstractValidator<AssignSportCommand>
{
    public AssignSportValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");
    }
}
