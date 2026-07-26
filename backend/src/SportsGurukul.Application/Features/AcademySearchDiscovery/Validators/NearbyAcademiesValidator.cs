using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.NearbyAcademies;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class NearbyAcademiesValidator : AbstractValidator<NearbyAcademiesQuery>
{
    public NearbyAcademiesValidator()
    {
        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage("Latitude is required.")
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage("Longitude is required.")
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.RadiusKm)
            .InclusiveBetween(0.1m, 500).WithMessage("RadiusKm must be between 0.1 and 500.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
    }
}
