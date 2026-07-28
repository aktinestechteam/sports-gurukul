using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.CalendarEvents;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class CalendarEventsValidator : AbstractValidator<CalendarEventsQuery>
{
    public CalendarEventsValidator()
    {
        RuleFor(x => x.FromDate)
            .LessThan(x => x.ToDate).WithMessage("From date must be less than to date.");
    }
}
