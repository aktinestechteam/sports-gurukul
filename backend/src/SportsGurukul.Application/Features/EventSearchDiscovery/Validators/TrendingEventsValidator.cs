using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.TrendingEvents;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class TrendingEventsValidator : AbstractValidator<TrendingEventsQuery>
{
    public TrendingEventsValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");
    }
}
