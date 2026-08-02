using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class CreatePromptTemplateCommandValidator : AbstractValidator<CreatePromptTemplateCommand>
{
    public CreatePromptTemplateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TemplateContent)
            .NotEmpty();
    }
}
