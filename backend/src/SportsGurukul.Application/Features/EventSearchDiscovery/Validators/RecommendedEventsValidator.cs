using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.RecommendedEvents;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class RecommendedEventsValidator : AbstractValidator<RecommendedEventsQuery>
{
    public RecommendedEventsValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m).WithMessage("Latitude must be between -90 and 90.")
            .When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m).WithMessage("Longitude must be between -180 and 180.")
            .When(x => x.Longitude.HasValue);
    }
}
