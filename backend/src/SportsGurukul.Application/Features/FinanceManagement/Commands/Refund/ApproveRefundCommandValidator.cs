using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public class ApproveRefundCommandValidator : AbstractValidator<ApproveRefundCommand>
{
    public ApproveRefundCommandValidator()
    {
        RuleFor(x => x.ApprovedBy).NotEmpty().WithMessage("Approver name is required");
    }
}
