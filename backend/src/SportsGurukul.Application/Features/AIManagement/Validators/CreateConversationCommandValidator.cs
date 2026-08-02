using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).When(x => x.Title is not null);
    }
}
