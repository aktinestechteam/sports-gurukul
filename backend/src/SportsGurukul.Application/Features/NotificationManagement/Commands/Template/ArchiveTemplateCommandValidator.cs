using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class ArchiveTemplateCommandValidator : AbstractValidator<ArchiveTemplateCommand>
{
    public ArchiveTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
