using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public class TransferWalletBalanceCommandValidator : AbstractValidator<TransferWalletBalanceCommand>
{
    public TransferWalletBalanceCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
        RuleFor(x => x.FromWalletId).NotEmpty().WithMessage("Source wallet is required");
        RuleFor(x => x.ToWalletId).NotEmpty().WithMessage("Destination wallet is required");
        RuleFor(x => x.FromWalletId).NotEqual(x => x.ToWalletId).WithMessage("Cannot transfer to the same wallet");
    }
}
