using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public class RequestRefundCommandValidator : AbstractValidator<RequestRefundCommand>
{
    public RequestRefundCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty().WithMessage("Payment is required");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
    }
}
