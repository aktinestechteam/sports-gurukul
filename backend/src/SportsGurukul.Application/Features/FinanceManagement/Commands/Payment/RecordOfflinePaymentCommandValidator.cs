using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class RecordOfflinePaymentCommandValidator : AbstractValidator<RecordOfflinePaymentCommand>
{
    public RecordOfflinePaymentCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
        RuleFor(x => x.InvoiceId).NotEmpty().WithMessage("Invoice is required");
        RuleFor(x => x.PaymentMethod).IsInEnum().WithMessage("Invalid payment method");
        RuleFor(x => x.PaidAt).NotEmpty().WithMessage("Payment date is required");
    }
}
