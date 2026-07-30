using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class CancelPaymentCommandValidator : AbstractValidator<CancelPaymentCommand>
{
    public CancelPaymentCommandValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Cancellation reason is required");
    }
}
