using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public class CreditWalletCommandValidator : AbstractValidator<CreditWalletCommand>
{
    public CreditWalletCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
    }
}
