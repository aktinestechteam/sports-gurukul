using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class ClonePromptCommandValidator : AbstractValidator<ClonePromptCommand>
{
    public ClonePromptCommandValidator()
    {
        RuleFor(x => x.NewName)
            .NotEmpty()
            .MaximumLength(200);
    }
}
