using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class CreateTemplateVersionCommandValidator : AbstractValidator<CreateTemplateVersionCommand>
{
    public CreateTemplateVersionCommandValidator()
    {
        RuleFor(x => x.TemplateId)
            .NotEmpty();

        RuleFor(x => x.SubjectTemplate)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.BodyTemplate)
            .NotEmpty();
    }
}
