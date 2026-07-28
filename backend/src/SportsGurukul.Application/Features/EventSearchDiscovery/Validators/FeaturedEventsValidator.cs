using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.FeaturedEvents;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class FeaturedEventsValidator : AbstractValidator<FeaturedEventsQuery>
{
    public FeaturedEventsValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");
    }
}
