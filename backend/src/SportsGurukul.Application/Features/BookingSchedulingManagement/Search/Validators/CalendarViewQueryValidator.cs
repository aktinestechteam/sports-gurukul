using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.CalendarView;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class CalendarViewQueryValidator : AbstractValidator<CalendarViewQuery>
{
    public CalendarViewQueryValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");
    }
}
