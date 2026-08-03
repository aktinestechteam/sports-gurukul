using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class RenameConversationCommandValidator : AbstractValidator<RenameConversationCommand>
{
    public RenameConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty().WithMessage("Conversation is required");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200).WithMessage("Title is required and must be at most 200 characters");
    }
}
