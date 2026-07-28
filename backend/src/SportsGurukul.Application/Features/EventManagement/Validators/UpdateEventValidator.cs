using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.UpdateEvent;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class UpdateEventValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.EventName)
            .MaximumLength(200).WithMessage("Event name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.EventName));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("Max participants must be greater than 0.")
            .When(x => x.MaxParticipants.HasValue);

        RuleFor(x => x.MinParticipants)
            .GreaterThan(0).WithMessage("Min participants must be greater than 0.")
            .When(x => x.MinParticipants.HasValue);

        RuleFor(x => x.RegistrationFee)
            .GreaterThanOrEqualTo(0).WithMessage("Registration fee must be non-negative.")
            .When(x => x.RegistrationFee.HasValue);

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}
