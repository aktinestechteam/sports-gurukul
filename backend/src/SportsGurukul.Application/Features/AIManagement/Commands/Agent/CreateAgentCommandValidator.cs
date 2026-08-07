using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Name is required and must be at most 100 characters");
        RuleFor(x => x.MaxIterations).GreaterThan(0).When(x => x.MaxIterations.HasValue).WithMessage("Max iterations must be positive");
        RuleFor(x => x.Temperature).InclusiveBetween(0, 2).When(x => x.Temperature.HasValue).WithMessage("Temperature must be between 0 and 2");
    }
}
