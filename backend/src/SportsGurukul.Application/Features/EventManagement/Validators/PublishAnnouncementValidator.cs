using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.PublishAnnouncement;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class PublishAnnouncementValidator : AbstractValidator<PublishAnnouncementCommand>
{
    public PublishAnnouncementValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(2000).WithMessage("Message must not exceed 2000 characters.");
    }
}
