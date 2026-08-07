using FluentValidation;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Audit;

public class WriteAuditLogCommandValidator : AbstractValidator<WriteAuditLogCommand>
{
    public WriteAuditLogCommandValidator()
    {
        RuleFor(x => x.EntityType).NotEmpty().MaximumLength(200).WithMessage("Entity type is required and must be at most 200 characters");
        RuleFor(x => x.DetailsJson).MaximumLength(8000).WithMessage("Details must be at most 8000 characters");
        RuleFor(x => x.BeforeJson).MaximumLength(8000).WithMessage("Before snapshot must be at most 8000 characters");
        RuleFor(x => x.AfterJson).MaximumLength(8000).WithMessage("After snapshot must be at most 8000 characters");
    }
}
