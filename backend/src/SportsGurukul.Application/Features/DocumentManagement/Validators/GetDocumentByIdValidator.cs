using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Queries.GetDocumentById;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class GetDocumentByIdValidator : AbstractValidator<GetDocumentByIdQuery>
{
    public GetDocumentByIdValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");
    }
}
