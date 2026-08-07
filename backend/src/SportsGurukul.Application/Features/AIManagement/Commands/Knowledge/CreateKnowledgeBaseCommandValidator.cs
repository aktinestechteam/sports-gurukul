using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class CreateKnowledgeBaseCommandValidator : AbstractValidator<CreateKnowledgeBaseCommand>
{
    public CreateKnowledgeBaseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithMessage("Name is required and must be at most 150 characters");
        RuleFor(x => x.ChunkSize).InclusiveBetween(128, 8192).WithMessage("Chunk size must be between 128 and 8192");
        RuleFor(x => x.ChunkOverlap).InclusiveBetween(0, 1024).WithMessage("Chunk overlap must be between 0 and 1024");
    }
}
