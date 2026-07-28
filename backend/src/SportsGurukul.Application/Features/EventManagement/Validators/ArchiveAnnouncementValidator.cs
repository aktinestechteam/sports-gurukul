using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.ArchiveAnnouncement;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class ArchiveAnnouncementValidator : AbstractValidator<ArchiveAnnouncementCommand>
{
    public ArchiveAnnouncementValidator()
    {
        RuleFor(x => x.AnnouncementId)
            .NotEmpty().WithMessage("Announcement ID is required.");
    }
}
