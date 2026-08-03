using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class AssignToolsCommandValidator : AbstractValidator<AssignToolsCommand>
{
    public AssignToolsCommandValidator()
    {
        RuleFor(x => x.AssistantId).NotEmpty().WithMessage("Assistant is required");
        RuleFor(x => x.ToolDefinitionIds).NotNull().WithMessage("Tool list is required");
    }
}
