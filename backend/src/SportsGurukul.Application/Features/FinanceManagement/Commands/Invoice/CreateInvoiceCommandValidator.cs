using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.LineItems).NotEmpty().WithMessage("At least one line item is required");
        RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow).When(x => x.DueDate.HasValue).WithMessage("Due date must be in the future");
    }
}
