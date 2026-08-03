using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class CreatePromptTemplateCommandValidator : AbstractValidator<CreatePromptTemplateCommand>
{
    public CreatePromptTemplateCommandValidator()
    {
        RuleFor(x => x.AssistantId).NotEmpty().WithMessage("Assistant is required");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithMessage("Name is required and must be at most 150 characters");
        RuleFor(x => x.TemplateText).NotEmpty().WithMessage("Template text is required");
    }
}
