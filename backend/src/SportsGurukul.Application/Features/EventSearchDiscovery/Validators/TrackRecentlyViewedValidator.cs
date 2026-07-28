using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.TrackRecentlyViewed;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class TrackRecentlyViewedValidator : AbstractValidator<TrackRecentlyViewedCommand>
{
    public TrackRecentlyViewedValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");
    }
}
