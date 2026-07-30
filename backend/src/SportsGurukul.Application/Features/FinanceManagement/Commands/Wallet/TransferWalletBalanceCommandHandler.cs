using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public class TransferWalletBalanceCommandHandler : IRequestHandler<TransferWalletBalanceCommand, Result<WalletDto>>
{
    private readonly IWalletService _walletService;

    public TransferWalletBalanceCommandHandler(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task<Result<WalletDto>> Handle(TransferWalletBalanceCommand request, CancellationToken cancellationToken)
    {
        return await _walletService.TransferBalanceAsync(request.FromWalletId, request.ToWalletId, request.Amount, request.Description, cancellationToken);
    }
}
