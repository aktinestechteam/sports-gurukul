using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreFacility;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class RestoreFacilityValidator : AbstractValidator<RestoreFacilityCommand>
{
    public RestoreFacilityValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");
    }
}
