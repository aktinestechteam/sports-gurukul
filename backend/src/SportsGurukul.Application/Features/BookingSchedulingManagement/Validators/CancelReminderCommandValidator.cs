using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelReminder;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class CancelReminderCommandValidator : AbstractValidator<CancelReminderCommand>
{
    public CancelReminderCommandValidator()
    {
        RuleFor(x => x.ReminderId)
            .NotEmpty().WithMessage("Reminder ID is required.");
    }
}
