using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class UpdateTemplateCommandValidator : AbstractValidator<UpdateTemplateCommand>
{
    public UpdateTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name is not null);

        RuleFor(x => x.SubjectTemplate)
            .MaximumLength(500)
            .When(x => x.SubjectTemplate is not null);
    }
}
