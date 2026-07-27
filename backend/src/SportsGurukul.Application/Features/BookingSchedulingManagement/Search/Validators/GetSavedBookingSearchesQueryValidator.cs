using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetSavedBookingSearches;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class GetSavedBookingSearchesQueryValidator : AbstractValidator<GetSavedBookingSearchesQuery>
{
    public GetSavedBookingSearchesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
