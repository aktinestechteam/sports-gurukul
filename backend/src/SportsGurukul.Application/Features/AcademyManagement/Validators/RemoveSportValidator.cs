using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveSport;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RemoveSportValidator : AbstractValidator<RemoveSportCommand>
{
    public RemoveSportValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");
    }
}
