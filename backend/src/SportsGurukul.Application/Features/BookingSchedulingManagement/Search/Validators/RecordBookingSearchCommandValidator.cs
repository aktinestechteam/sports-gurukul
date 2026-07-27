using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.RecordBookingSearch;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class RecordBookingSearchCommandValidator : AbstractValidator<RecordBookingSearchCommand>
{
    public RecordBookingSearchCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Search term is required.")
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.");
    }
}
