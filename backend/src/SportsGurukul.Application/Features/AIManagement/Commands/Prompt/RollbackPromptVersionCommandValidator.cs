using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class RollbackPromptVersionCommandValidator : AbstractValidator<RollbackPromptVersionCommand>
{
    public RollbackPromptVersionCommandValidator()
    {
        RuleFor(x => x.PromptTemplateId).NotEmpty().WithMessage("Prompt template is required");
        RuleFor(x => x.VersionNumber).GreaterThanOrEqualTo(1).WithMessage("Version number must be at least 1");
    }
}
