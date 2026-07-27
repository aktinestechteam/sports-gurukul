using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.DeleteSavedBookingSearch;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class DeleteSavedBookingSearchCommandValidator : AbstractValidator<DeleteSavedBookingSearchCommand>
{
    public DeleteSavedBookingSearchCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.SavedSearchId)
            .NotEmpty().WithMessage("Saved search ID is required.");
    }
}
