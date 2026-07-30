using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public class CancelInvoiceCommandValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceCommandValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Cancellation reason is required");
    }
}
