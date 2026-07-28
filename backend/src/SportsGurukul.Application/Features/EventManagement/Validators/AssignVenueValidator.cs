using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.AssignVenue;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class AssignVenueValidator : AbstractValidator<AssignVenueCommand>
{
    public AssignVenueValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.VenueId)
            .NotEmpty().WithMessage("Venue ID is required.");
    }
}
