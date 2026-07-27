using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingStatistics;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetBookingStatisticsQueryValidator : AbstractValidator<GetBookingStatisticsQuery>
{
    public GetBookingStatisticsQueryValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after start date.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
