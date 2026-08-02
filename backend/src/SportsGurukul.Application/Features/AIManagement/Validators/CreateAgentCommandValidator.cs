using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Agent;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
