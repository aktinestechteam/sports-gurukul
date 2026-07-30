using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ChannelType)
            .IsInEnum();

        RuleFor(x => x.SubjectTemplate)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.BodyTemplate)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
