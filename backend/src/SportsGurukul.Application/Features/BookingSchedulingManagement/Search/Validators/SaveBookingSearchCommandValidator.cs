using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.SaveBookingSearch;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class SaveBookingSearchCommandValidator : AbstractValidator<SaveBookingSearchCommand>
{
    public SaveBookingSearchCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Search name is required.")
            .MaximumLength(100).WithMessage("Search name must not exceed 100 characters.");
    }
}
