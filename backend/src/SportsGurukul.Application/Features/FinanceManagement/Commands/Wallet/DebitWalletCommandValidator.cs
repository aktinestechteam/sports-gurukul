using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public class DebitWalletCommandValidator : AbstractValidator<DebitWalletCommand>
{
    public DebitWalletCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
    }
}
