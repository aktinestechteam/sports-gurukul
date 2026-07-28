using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.UpcomingEvents;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class UpcomingEventsValidator : AbstractValidator<UpcomingEventsQuery>
{
    public UpcomingEventsValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");
    }
}
