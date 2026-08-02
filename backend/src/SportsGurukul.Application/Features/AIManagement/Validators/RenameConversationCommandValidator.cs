using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class RenameConversationCommandValidator : AbstractValidator<RenameConversationCommand>
{
    public RenameConversationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}
