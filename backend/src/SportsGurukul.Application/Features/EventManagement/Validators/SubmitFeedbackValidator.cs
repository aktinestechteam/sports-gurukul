using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class SubmitFeedbackValidator : AbstractValidator<SubmitFeedbackCommand>
{
    public SubmitFeedbackValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.OverallRating)
            .InclusiveBetween(1, 5).WithMessage("Overall rating must be between 1 and 5.");

        RuleFor(x => x.ContentRating)
            .InclusiveBetween(1, 5).WithMessage("Content rating must be between 1 and 5.")
            .When(x => x.ContentRating.HasValue);

        RuleFor(x => x.SpeakerRating)
            .InclusiveBetween(1, 5).WithMessage("Speaker rating must be between 1 and 5.")
            .When(x => x.SpeakerRating.HasValue);

        RuleFor(x => x.VenueRating)
            .InclusiveBetween(1, 5).WithMessage("Venue rating must be between 1 and 5.")
            .When(x => x.VenueRating.HasValue);

        RuleFor(x => x.OrganizationRating)
            .InclusiveBetween(1, 5).WithMessage("Organization rating must be between 1 and 5.")
            .When(x => x.OrganizationRating.HasValue);

        RuleFor(x => x.Comments)
            .MaximumLength(2000).WithMessage("Comments must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comments));
    }
}
