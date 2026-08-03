using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.AssistantId).NotEmpty().WithMessage("Assistant is required");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200).WithMessage("Title is required and must be at most 200 characters");
    }
}
