using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ScheduleReminder;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class ScheduleReminderCommandValidator : AbstractValidator<ScheduleReminderCommand>
{
    public ScheduleReminderCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.ReminderMinutesBefore)
            .GreaterThan(0).WithMessage("Reminder minutes must be greater than zero.")
            .LessThanOrEqualTo(10080).WithMessage("Reminder minutes must not exceed 10080 (7 days).");

        RuleFor(x => x.Channel)
            .MaximumLength(50).WithMessage("Channel must not exceed 50 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}
