using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateRecurringBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class CreateRecurringBookingCommandValidator : AbstractValidator<CreateRecurringBookingCommand>
{
    public CreateRecurringBookingCommandValidator()
    {
        RuleFor(x => x.BookingType)
            .NotEmpty().WithMessage("Booking type is required.")
            .MaximumLength(50).WithMessage("Booking type must not exceed 50 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.");

        RuleFor(x => x.RecurrenceType)
            .NotEmpty().WithMessage("Recurrence type is required.")
            .MaximumLength(50).WithMessage("Recurrence type must not exceed 50 characters.");

        RuleFor(x => x.OccurrenceCount)
            .GreaterThan(0).WithMessage("Occurrence count must be greater than zero.")
            .LessThanOrEqualTo(52).WithMessage("Occurrence count must not exceed 52.")
            .When(x => x.OccurrenceCount.HasValue);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x)
            .Must(x => x.StartTime < x.EndTime)
            .WithMessage("Start time must be before end time.");

        RuleFor(x => x)
            .Must(x => x.OccurrenceCount.HasValue || x.EndDate.HasValue)
            .WithMessage("Either occurrence count or end date must be specified.");
    }
}
