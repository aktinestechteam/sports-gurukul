using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class SummarizeConversationCommandValidator : AbstractValidator<SummarizeConversationCommand>
{
    public SummarizeConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty().WithMessage("Conversation is required");
        RuleFor(x => x.Summary).NotEmpty().WithMessage("Summary is required");
    }
}
