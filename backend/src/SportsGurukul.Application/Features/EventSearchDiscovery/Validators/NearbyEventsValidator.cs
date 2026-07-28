using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.NearbyEvents;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class NearbyEventsValidator : AbstractValidator<NearbyEventsQuery>
{
    public NearbyEventsValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(x => x.RadiusKm)
            .InclusiveBetween(0.1m, 500m).WithMessage("Radius must be between 0.1 and 500 km.");
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");
    }
}
