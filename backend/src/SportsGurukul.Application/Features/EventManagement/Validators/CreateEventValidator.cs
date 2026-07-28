using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class CreateEventValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("Event name is required.")
            .MaximumLength(200).WithMessage("Event name must not exceed 200 characters.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");

        RuleFor(x => x.EventTypeId)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

        RuleFor(x => x.RegistrationOpenDate)
            .NotEmpty().WithMessage("Registration open date is required.");

        RuleFor(x => x.RegistrationCloseDate)
            .NotEmpty().WithMessage("Registration close date is required.")
            .LessThan(x => x.StartDate).WithMessage("Registration close date must be before start date.");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("Max participants must be greater than 0.")
            .When(x => x.MaxParticipants.HasValue);

        RuleFor(x => x.MinParticipants)
            .GreaterThan(0).WithMessage("Min participants must be greater than 0.")
            .When(x => x.MinParticipants.HasValue);

        RuleFor(x => x.RegistrationFee)
            .GreaterThanOrEqualTo(0).WithMessage("Registration fee must be non-negative.")
            .When(x => x.RegistrationFee.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}
