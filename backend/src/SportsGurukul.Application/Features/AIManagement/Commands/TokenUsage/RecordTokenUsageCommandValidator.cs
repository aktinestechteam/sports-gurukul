using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.TokenUsage;

public class RecordTokenUsageCommandValidator : AbstractValidator<RecordTokenUsageCommand>
{
    public RecordTokenUsageCommandValidator()
    {
        RuleFor(x => x.InputTokens).GreaterThanOrEqualTo(0).WithMessage("Input tokens cannot be negative");
        RuleFor(x => x.OutputTokens).GreaterThanOrEqualTo(0).WithMessage("Output tokens cannot be negative");
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0).When(x => x.Cost.HasValue).WithMessage("Cost cannot be negative");
        RuleFor(x => x.LatencyMs).GreaterThanOrEqualTo(0).When(x => x.LatencyMs.HasValue).WithMessage("Latency cannot be negative");
    }
}
