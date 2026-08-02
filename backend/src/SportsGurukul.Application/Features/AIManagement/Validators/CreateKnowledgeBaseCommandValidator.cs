using FluentValidation;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

namespace SportsGurukul.Application.Features.AIManagement.Validators;

public class CreateKnowledgeBaseCommandValidator : AbstractValidator<CreateKnowledgeBaseCommand>
{
    public CreateKnowledgeBaseCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
