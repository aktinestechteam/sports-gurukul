using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.SendReminder;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class SendReminderCommandValidator : AbstractValidator<SendReminderCommand>
{
    public SendReminderCommandValidator()
    {
        RuleFor(x => x.ReminderId)
            .NotEmpty().WithMessage("Reminder ID is required.");

        RuleFor(x => x.OverrideChannel)
            .MaximumLength(50).WithMessage("Override channel must not exceed 50 characters.");
    }
}
