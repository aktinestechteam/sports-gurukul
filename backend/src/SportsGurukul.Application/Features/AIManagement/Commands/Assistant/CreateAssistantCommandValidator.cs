using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class CreateAssistantCommandValidator : AbstractValidator<CreateAssistantCommand>
{
    public CreateAssistantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Name is required and must be at most 100 characters");
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150).WithMessage("Display name is required and must be at most 150 characters");
        RuleFor(x => x.MaxTokens).GreaterThan(0).When(x => x.MaxTokens.HasValue).WithMessage("Max tokens must be positive");
        RuleFor(x => x.Temperature).InclusiveBetween(0, 2).When(x => x.Temperature.HasValue).WithMessage("Temperature must be between 0 and 2");
    }
}
