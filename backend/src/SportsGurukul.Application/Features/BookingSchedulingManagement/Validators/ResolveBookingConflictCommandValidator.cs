using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ResolveBookingConflict;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class ResolveBookingConflictCommandValidator : AbstractValidator<ResolveBookingConflictCommand>
{
    public ResolveBookingConflictCommandValidator()
    {
        RuleFor(x => x.ConflictId)
            .NotEmpty().WithMessage("Conflict ID is required.");

        RuleFor(x => x.ResolutionNotes)
            .NotEmpty().WithMessage("Resolution notes are required.")
            .MaximumLength(1000).WithMessage("Resolution notes must not exceed 1000 characters.");
    }
}
