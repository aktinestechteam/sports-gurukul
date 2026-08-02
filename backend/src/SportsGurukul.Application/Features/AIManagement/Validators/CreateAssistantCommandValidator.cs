using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class CreateAssistantCommandValidator : AbstractValidator<CreateAssistantCommand>
{
    public CreateAssistantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SystemPrompt)
            .MaximumLength(8000).When(x => x.SystemPrompt is not null);
    }
}
