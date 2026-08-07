using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty().WithMessage("Conversation is required");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Message content is required");
        RuleFor(x => x.InputTokenCount).GreaterThanOrEqualTo(0).When(x => x.InputTokenCount.HasValue)
            .WithMessage("Input token count cannot be negative");
        RuleFor(x => x.OutputTokenCount).GreaterThanOrEqualTo(0).When(x => x.OutputTokenCount.HasValue)
            .WithMessage("Output token count cannot be negative");
    }
}
