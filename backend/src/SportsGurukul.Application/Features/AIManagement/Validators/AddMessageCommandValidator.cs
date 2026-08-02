using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty();

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(100000);

        RuleFor(x => x.Role)
            .IsInEnum();
    }
}
