using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class PublishTemplateCommandValidator : AbstractValidator<PublishTemplateCommand>
{
    public PublishTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
