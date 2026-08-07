using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class PublishPromptTemplateCommandValidator : AbstractValidator<PublishPromptTemplateCommand>
{
    public PublishPromptTemplateCommandValidator()
    {
        RuleFor(x => x.PromptTemplateId).NotEmpty().WithMessage("Prompt template is required");
    }
}
