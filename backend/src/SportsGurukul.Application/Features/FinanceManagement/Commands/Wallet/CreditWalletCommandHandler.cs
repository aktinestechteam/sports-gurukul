using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public class CreditWalletCommandHandler : IRequestHandler<CreditWalletCommand, Result<WalletDto>>
{
    private readonly IWalletService _walletService;

    public CreditWalletCommandHandler(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task<Result<WalletDto>> Handle(CreditWalletCommand request, CancellationToken cancellationToken)
    {
        return await _walletService.CreditWalletAsync(request.WalletId, request.Amount, request.Reference, request.Description, cancellationToken);
    }
}
