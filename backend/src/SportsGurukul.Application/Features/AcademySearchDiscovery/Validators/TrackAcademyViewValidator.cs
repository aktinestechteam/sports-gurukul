using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.TrackAcademyView;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class TrackAcademyViewValidator : AbstractValidator<TrackAcademyViewCommand>
{
    public TrackAcademyViewValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Source is required.")
            .MaximumLength(50).WithMessage("Source must not exceed 50 characters.");
    }
}
