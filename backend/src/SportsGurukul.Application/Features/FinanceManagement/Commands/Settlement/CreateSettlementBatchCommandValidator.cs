using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;

public class CreateSettlementBatchCommandValidator : AbstractValidator<CreateSettlementBatchCommand>
{
    public CreateSettlementBatchCommandValidator()
    {
        RuleFor(x => x.PaymentIds).NotEmpty().WithMessage("At least one payment is required");
    }
}
