using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class AssignKnowledgeBaseCommandValidator : AbstractValidator<AssignKnowledgeBaseCommand>
{
    public AssignKnowledgeBaseCommandValidator()
    {
        RuleFor(x => x.AssistantId).NotEmpty().WithMessage("Assistant is required");
        RuleFor(x => x.KnowledgeBaseIds).NotNull().WithMessage("Knowledge base list is required");
    }
}
