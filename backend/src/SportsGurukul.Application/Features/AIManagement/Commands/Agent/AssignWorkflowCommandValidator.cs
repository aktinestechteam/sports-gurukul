using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class AssignWorkflowCommandValidator : AbstractValidator<AssignWorkflowCommand>
{
    public AssignWorkflowCommandValidator()
    {
        RuleFor(x => x.AgentId).NotEmpty().WithMessage("Agent is required");
        RuleFor(x => x.WorkflowId).NotEmpty().WithMessage("Workflow is required");
    }
}
