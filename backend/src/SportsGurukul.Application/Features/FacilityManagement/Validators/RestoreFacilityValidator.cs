using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.RestoreFacility;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class RestoreFacilityValidator : AbstractValidator<RestoreFacilityCommand>
{
    public RestoreFacilityValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");
    }
}
