using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetResourceCalendar;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class GetResourceCalendarQueryValidator : AbstractValidator<GetResourceCalendarQuery>
{
    public GetResourceCalendarQueryValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.ResourceType)
            .NotEmpty().WithMessage("Resource type is required.")
            .Must(rt => new[] { "facility", "coach" }.Contains(rt.ToLowerInvariant()))
            .WithMessage("Resource type must be 'facility' or 'coach'.");

        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("EndDate must be after StartDate.");
    }
}
