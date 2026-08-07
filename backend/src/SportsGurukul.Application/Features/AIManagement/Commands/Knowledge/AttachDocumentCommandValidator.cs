using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class AttachDocumentCommandValidator : AbstractValidator<AttachDocumentCommand>
{
    public AttachDocumentCommandValidator()
    {
        RuleFor(x => x.KnowledgeBaseId).NotEmpty().WithMessage("Knowledge base is required");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300).WithMessage("Document title is required and must be at most 300 characters");
    }
}
