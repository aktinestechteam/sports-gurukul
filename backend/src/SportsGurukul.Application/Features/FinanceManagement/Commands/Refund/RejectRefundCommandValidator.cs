using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public class RejectRefundCommandValidator : AbstractValidator<RejectRefundCommand>
{
    public RejectRefundCommandValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Rejection reason is required");
    }
}
